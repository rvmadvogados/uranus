using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Uranus.AuthApi.Helpers;
using Uranus.AuthApi.Models;
using Uranus.AuthApi.Services;

namespace Uranus.AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ITwoFactorService _twoFactorService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IEmailService emailService,
        ITwoFactorService twoFactorService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
        _twoFactorService = twoFactorService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuario ou senha invalidos" });
        }

        user = await _userManager.FindByNameAsync(model.Username);


        if (user.AccessFailedCount > 0 && user.LastFailedLoginDate.HasValue)
        {
            int autoResetMinutes = _configuration.GetValue<int>("Lockout:AutoResetMinutes", 15);
            var tempoDecorrido = DateTime.Now - user.LastFailedLoginDate.Value;
            if (tempoDecorrido.TotalMinutes >= autoResetMinutes)
            {
                user.AccessFailedCount = 0;
                user.LockoutEnd = null;
                user.LastFailedLoginDate = null;
                await _userManager.UpdateAsync(user);
            }
        }
        else if (user.AccessFailedCount > 0 && user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTimeOffset.Now)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.LastFailedLoginDate = null;
            await _userManager.UpdateAsync(user);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            var lockoutEnd = user.LockoutEnd;
            if (lockoutEnd.HasValue)
            {
                var timeRemaining = lockoutEnd.Value - DateTime.Now;
                int minutosRestantes = (int)Math.Ceiling(timeRemaining.TotalMinutes);

                if (minutosRestantes > 0)
                {
                    return Unauthorized(new
                    {
                        message = $"Usuario bloqueado. Tente novamente em {minutosRestantes} minuto(s).",
                        blockedUntil = lockoutEnd,
                        minutesRemaining = minutosRestantes,
                        isLocked = true
                    });
                }
                else
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(user);
                }
            }
            else
            {
                return Unauthorized(new { message = "Usuario bloqueado temporariamente. Tente novamente em alguns minutos." });
            }
        }

        bool hasPassword = await _userManager.HasPasswordAsync(user);
        bool passwordValid = false;

        if (!hasPassword && !string.IsNullOrEmpty(user.LegacyMd5Hash))
        {
            if (Md5Helper.VerificarHashMd5(model.Password, user.LegacyMd5Hash))
            {
                passwordValid = true;
                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                if (addPasswordResult.Succeeded)
                {
                    user.LegacyMd5Hash = null;
                    await _userManager.UpdateAsync(user);
                }
            }
        }
        else if (hasPassword)
        {
            passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        }

        if (!passwordValid)
        {
            user.AccessFailedCount++;
            user.LastFailedLoginDate = DateTime.Now;

            if (user.AccessFailedCount >= _userManager.Options.Lockout.MaxFailedAccessAttempts)
            {
                user.LockoutEnd = DateTimeOffset.Now.Add(_userManager.Options.Lockout.DefaultLockoutTimeSpan);

                try
                {
                    _ = Task.Run(async () => await _emailService.EnviarNotificacaoBloqueioAsync(user.UserName, user.Email, user.LockoutEnd?.DateTime));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EMAIL BLOQUEIO ERRO] Erro ao iniciar envio: {ex.Message}");
                }
            }

            var result = await _userManager.UpdateAsync(user);

            int maxAttempts = (int)_userManager.Options.Lockout.MaxFailedAccessAttempts;
            int attemptsRemaining = Math.Max(0, maxAttempts - user.AccessFailedCount);

            bool isLockedNow = user.AccessFailedCount >= maxAttempts && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now;

            return Unauthorized(new
            {
                message = isLockedNow ? "Sua conta foi bloqueada temporariamente por excesso de tentativas. Tente novamente em alguns minutos." : "Usuario ou senha invalidos",
                attemptsRemaining = isLockedNow ? 0 : attemptsRemaining,
                isLocked = isLockedNow
            });
        }

        if (user.AccessFailedCount > 0 || user.LockoutEnd.HasValue)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.LastFailedLoginDate = null;
            await _userManager.UpdateAsync(user);
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));

        if (isTwoFactorEnabled && !hasAuthenticator)
        {
            var setupToken = await GenerateJwtToken(user, new List<string>(), new List<Claim>());

            return Ok(new
            {
                requiresSetup = true,
                setupToken = setupToken,
                message = "Voce precisa configurar a autenticacao de dois fatores"
            });
        }

        if (isTwoFactorEnabled && hasAuthenticator)
        {
            int twoFactorValidityMinutes = _configuration.GetValue<int>("TwoFactor:ValidityMinutes", 15);


            if (_twoFactorService.IsTwoFactorStillValid(user, twoFactorValidityMinutes))
            {
                return await GenerateLoginResponse(user);
            }

            var preferredMethod = user.PreferredTwoFactorMethod ?? "App";
            var hasEmail = !string.IsNullOrEmpty(user.Email);
            var maskedEmail = hasEmail ? MaskEmail(user.Email) : null;

            return Ok(new
            {
                requiresTwoFactor = true,
                preferredMethod = preferredMethod,
                hasAuthenticator = true,
                hasEmail = hasEmail,
                maskedEmail = maskedEmail,
                message = "Por favor, forneca o codigo de autenticacao de dois fatores"
            });
        }

        return await GenerateLoginResponse(user);
    }

    [HttpPost("login-2fa")]
    public async Task<IActionResult> LoginWith2FA([FromBody] LoginWith2FARequest model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Usuario nao encontrado" });
        }

        user = await _userManager.FindByNameAsync(model.Username);

        if (user.AccessFailedCount > 0 && user.LastFailedLoginDate.HasValue)
        {
            int autoResetMinutes = _configuration.GetValue<int>("Lockout:AutoResetMinutes", 15);
            var tempoDecorrido = DateTime.Now - user.LastFailedLoginDate.Value;
            if (tempoDecorrido.TotalMinutes >= autoResetMinutes)
            {
                user.AccessFailedCount = 0;
                user.LockoutEnd = null;
                user.LastFailedLoginDate = null;
                await _userManager.UpdateAsync(user);
            }
        }
        else if (user.AccessFailedCount > 0 && user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTimeOffset.Now)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.LastFailedLoginDate = null;
            await _userManager.UpdateAsync(user);
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            var lockoutEnd = user.LockoutEnd;
            if (lockoutEnd.HasValue)
            {
                var timeRemaining = lockoutEnd.Value - DateTime.Now;
                int minutosRestantes = (int)Math.Ceiling(timeRemaining.TotalMinutes);

                if (minutosRestantes > 0)
                {
                    return Unauthorized(new
                    {
                        message = $"Usuario bloqueado. Tente novamente em {minutosRestantes} minuto(s).",
                        blockedUntil = lockoutEnd,
                        minutesRemaining = minutosRestantes,
                        isLocked = true
                    });
                }
                else
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(user);
                }
            }
            else
            {
                return Unauthorized(new { message = "Usuario bloqueado temporariamente. Tente novamente em alguns minutos." });
            }
        }

        bool passwordValid = false;
        bool hasPassword = await _userManager.HasPasswordAsync(user);

        if (!hasPassword && !string.IsNullOrEmpty(user.LegacyMd5Hash))
        {
            passwordValid = Md5Helper.VerificarHashMd5(model.Password, user.LegacyMd5Hash);
        }
        else if (hasPassword)
        {
            passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        }

        if (!passwordValid)
        {
            user.AccessFailedCount++;
            user.LastFailedLoginDate = DateTime.Now;

            if (user.AccessFailedCount >= _userManager.Options.Lockout.MaxFailedAccessAttempts)
            {
                user.LockoutEnd = DateTimeOffset.Now.Add(_userManager.Options.Lockout.DefaultLockoutTimeSpan);

                try
                {
                    _ = Task.Run(async () => await _emailService.EnviarNotificacaoBloqueioAsync(user.UserName, user.Email, user.LockoutEnd?.DateTime));
                    System.Diagnostics.Debug.WriteLine($"[EMAIL BLOQUEIO2FA] Notificacao de bloqueio iniciada para {user.UserName}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EMAIL BLOQUEIO2FA ERRO] Erro ao iniciar envio: {ex.Message}");
                }
            }

            await _userManager.UpdateAsync(user);

            int maxAttempts = (int)_userManager.Options.Lockout.MaxFailedAccessAttempts;
            int attemptsRemaining = Math.Max(0, maxAttempts - user.AccessFailedCount);

            return Unauthorized(new
            {
                message = "Usuario ou senha invalidos",
                attemptsRemaining = attemptsRemaining,
                isLocked = false
            });
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            return BadRequest(new { message = "Autenticacao de dois fatores nao esta habilitada para este usuario" });
        }

        bool is2FACodeValid = false;

        is2FACodeValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            model.TwoFactorCode);

        if (!is2FACodeValid)
        {
            is2FACodeValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                model.TwoFactorCode);
        }

        if (!is2FACodeValid)
        {
            var isRecoveryCodeValid = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, model.TwoFactorCode);

            if (!isRecoveryCodeValid.Succeeded)
            {
                user.AccessFailedCount++;
                user.LastFailedLoginDate = DateTime.Now;

                if (user.AccessFailedCount >= _userManager.Options.Lockout.MaxFailedAccessAttempts)
                {
                    user.LockoutEnd = DateTimeOffset.Now.Add(_userManager.Options.Lockout.DefaultLockoutTimeSpan);
                }

                await _userManager.UpdateAsync(user);
                return Unauthorized(new { message = "Codigo de autenticacao invalido" });
            }

            is2FACodeValid = true;
        }

        if (user.AccessFailedCount > 0 || user.LockoutEnd.HasValue)
        {
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            user.LastFailedLoginDate = null;
            await _userManager.UpdateAsync(user);
        }

        await _twoFactorService.SaveLastTwoFactorConfirmedAsync(user);
        return await GenerateLoginResponse(user);
    }

    private async Task<IActionResult> GenerateLoginResponse(ApplicationUser user)
    {
        user.LastLoginDate = DateTime.Now;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                roleClaims.AddRange(claims);
            }
        }

        var allClaims = userClaims.Concat(roleClaims)
            .GroupBy(c => new { c.Type, c.Value })
            .Select(g => g.First())
            .ToList();

        var token = await GenerateJwtToken(user, roles, allClaims);

        return Ok(new
        {
            token,
            usuario = new
            {
                nome = user.UserName,
                roles = roles,
                claims = allClaims.Select(c => new { type = c.Type, value = c.Value })
            }
        });
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user, IList<string> roles, IList<Claim> claims)
    {
        var jwtClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        jwtClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        jwtClaims.AddRange(claims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: jwtClaims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "";

        var parts = email.Split('@');
        if (parts.Length != 2)
            return email;

        var username = parts[0];
        var domain = parts[1];

        if (username.Length <= 2)
            return $"{username}@{domain}";

        var visibleChars = Math.Min(2, username.Length / 3);
        var maskedPart = new string('*', username.Length - visibleChars);

        return $"{username.Substring(0, visibleChars)}{maskedPart}@{domain}";
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.UserName,
            u.Email,
            u.LockoutEnabled,
            u.LockoutEnd,
            u.AccessFailedCount,
            IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.Now,
            LastLoginDate = u.LastLoginDate
        }).ToList();

        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);
        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.AccessFailedCount,
            user.LockoutEnabled,
            user.LockoutEnd,
            IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now,
            LastLoginDate = user.LastLoginDate,
            TwoFactorEnabled = isTwoFactorEnabled,
            HasAuthenticator = hasAuthenticator,
            Roles = roles,
            Claims = claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpGet("users-by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.AccessFailedCount,
            user.LockoutEnabled,
            user.LockoutEnd,
            isLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now,
            lastLoginDate = user.LastLoginDate,
            twoFactorEnabled = isTwoFactorEnabled,
            roles = roles
        });
    }

    [HttpPost("users/{id}/reset-failed-attempts")]
    public async Task<IActionResult> ResetFailedAttempts(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userManager.ResetAccessFailedCountAsync(user);
        if (result.Succeeded)
        {
            return Ok(new { message = "Contador de tentativas falhadas foi resetado" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("users/{id}/unlock")]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var result = await _userManager.SetLockoutEndDateAsync(user, null);
        if (result.Succeeded)
        {
            return Ok(new { message = "Usuario foi desbloqueado" });
        }

        return BadRequest(result.Errors);
    }
}
