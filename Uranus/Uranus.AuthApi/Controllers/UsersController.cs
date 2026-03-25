using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uranus.AuthApi.Models;


[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { error = "UserName é obrigatório." });

        if (string.IsNullOrWhiteSpace(model.NewPassword))
            return BadRequest(new { error = "NewPassword é obrigatório." });

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return NotFound(new { error = "Usuário não encontrado." });

        try
        {
            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                    return BadRequest(new { error = $"Erro ao remover senha antiga: {errors}" });
                }
            }

            var addResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                return BadRequest(new { error = $"Erro ao definir nova senha: {errors}" });
            }

            return Ok(new
            {
                success = true,
                message = "Senha alterada com sucesso no sistema de autenticação."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro interno ao alterar senha: {ex.Message}" });
        }
    }

    [HttpGet("check-email")]
    [Authorize]
    public async Task<IActionResult> CheckEmailUniqueness([FromQuery] string email, [FromQuery] string excludeUsername = null)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Ok(new { exists = false });
        }

        var normalizedEmail = email.Trim().ToLower();

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);

        if (user != null)
        {
            if (!string.IsNullOrWhiteSpace(excludeUsername) && user.UserName.Equals(excludeUsername, StringComparison.OrdinalIgnoreCase))
            {
                return Ok(new { exists = false });
            }

            return Ok(new { exists = true, usedBy = user.UserName });
        }

        return Ok(new { exists = false });
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { message = "UserName é obrigatório." });

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        if (model.Email != null) // null significa "não alterar", "" significa "apagar"
        {
            var emailToSet = model.Email.Trim();

            if (string.IsNullOrWhiteSpace(emailToSet))
            {
                user.Email = null;
                user.NormalizedEmail = null;
                user.EmailConfirmed = false;

                if (user.TwoFactorEnabled)
                {
                    user.TwoFactorEnabled = false;
                }
            }
            else
            {
                var normalizedNewEmail = emailToSet.ToLower();
                var emailExists = await _userManager.Users
            .AnyAsync(u => u.Email != null &&
            u.Email.ToLower() == normalizedNewEmail &&
             u.UserName != model.UserName);

                if (emailExists)
                {
                    return BadRequest(new { message = "Este e-mail já está sendo usado por outro usuário." });
                }

                user.Email = emailToSet.ToLower();
                user.NormalizedEmail = emailToSet.ToUpper();
                user.EmailConfirmed = false; 
            }

            var emailUpdateResult = await _userManager.UpdateAsync(user);
            if (!emailUpdateResult.Succeeded)
            {
                var errors = string.Join("; ", emailUpdateResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Erro ao atualizar e-mail: {errors}" });
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Erro ao remover senha antiga: {errors}" });
                }
            }

            var passwordResult = await _userManager.AddPasswordAsync(user, model.Password);
            if (!passwordResult.Succeeded)
            {
                var errors = string.Join("; ", passwordResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Erro ao definir nova senha: {errors}" });
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Role))
        {
            var role = await _roleManager.FindByNameAsync(model.Role);
            if (role == null)
                return BadRequest(new { message = $"Perfil '{model.Role}' não existe." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = $"Erro ao remover roles antigas: {errors}" });
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Erro ao adicionar role: {errors}" });
            }
        }

        return Ok(new
        {
            message = "Usuário atualizado com sucesso.",
            emailRemoved = model.Email != null && string.IsNullOrWhiteSpace(model.Email)
        });
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] UpdateUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { message = "UserName é obrigatório." });
        if (string.IsNullOrWhiteSpace(model.Password))
            return BadRequest(new { message = "Password é obrigatório." });
        if (string.IsNullOrWhiteSpace(model.Role))
            return BadRequest(new { message = "Role é obrigatório." });

        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
            return BadRequest(new { message = "Usuário já existe." });

        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var normalizedEmail = model.Email.Trim().ToLower();
            var emailExists = await _userManager.Users
             .AnyAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                return BadRequest(new { message = "Este e-mail já está sendo usado por outro usuário." });
            }
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Trim().ToLower() : null,
            NormalizedEmail = !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Trim().ToUpper() : null,
            EmailConfirmed = false
        };

        try
        {
            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Erro ao criar usuário: {errors}" });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest(new { message = $"Perfil '{model.Role}' não existe." });

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Erro ao adicionar role: {errors}" });
            }

            return Ok(new { message = "Usuário criado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Exceção durante criação: {ex.Message}" });
        }
    }

    [HttpPost("unlock")]
    [Authorize]
    public async Task<IActionResult> Unlock([FromBody] UnlockUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { error = "UserName é obrigatório." });

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return NotFound(new { error = "Usuário não encontrado." });

        try
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            if (user.LockoutEnabled)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new { error = $"Erro ao desbloquear usuário: {errors}" });
            }

            return Ok(new
            {
                result = "success",
                message = "Usuário desbloqueado com sucesso.",
                accessFailedCount = 0,
                lockoutEnd = (string)null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro interno ao desbloquear usuário: {ex.Message}" });
        }
    }

    [HttpPost("enable")]
    [Authorize]
    public async Task<IActionResult> Enable([FromBody] UpdateUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { error = "UserName é obrigatório." });

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return NotFound(new { error = "Usuário não encontrado." });

        try
        {
            if (user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                user.LockoutEnd = null;
                user.LockoutEnabled = false;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { error = $"Erro ao ativar usuário: {errors}" });
                }

                return Ok(new
                {
                    result = "success",
                    message = "Usuário ativado com sucesso.",
                    isActive = true
                });
            }

            return Ok(new
            {
                result = "success",
                message = "Usuário já está ativo.",
                isActive = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro interno ao ativar usuário: {ex.Message}" });
        }
    }

    [HttpPost("disable")]
    [Authorize]
    public async Task<IActionResult> Disable([FromBody] UpdateUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.UserName))
            return BadRequest(new { error = "UserName é obrigatório." });

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return NotFound(new { error = "Usuário não encontrado." });

        try
        {
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.LockoutEnabled = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest(new { error = $"Erro ao desativar usuário: {errors}" });
            }

            return Ok(new
            {
                result = "success",
                message = "Usuário desativado com sucesso.",
                isActive = false
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Erro interno ao desativar usuário: {ex.Message}" });
        }
    }

    [HttpPost("random-password")]
    [Authorize]
    public IActionResult RandomPassword()
    {
        int length = 8;
        string lowercase = "abcdefghijklmnopqrstuvwxyz";
        string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string digits = "1234567890";
        string specials = "!#$%^&*()-+<>";
        string all = lowercase + uppercase + digits + specials;

        var rand = new Random();
        string pass = "";
        pass += lowercase[rand.Next(lowercase.Length)];
        pass += uppercase[rand.Next(uppercase.Length)];
        pass += digits[rand.Next(digits.Length)];
        pass += specials[rand.Next(specials.Length)];

        for (int x = pass.Length; x < length; x++)
        {
            pass += all[rand.Next(all.Length)];
        }

        pass = new string(pass.ToCharArray().OrderBy(c => Guid.NewGuid()).ToArray());

        return Ok(new { password = pass });
    }

    [HttpGet("{username}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username é obrigatório.");

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        System.Diagnostics.Debug.WriteLine($"[GetUser] Retornando dados do usuário {username}. " +
            $"LockoutEnd: {user.LockoutEnd}, Type: {user.LockoutEnd?.GetType().Name}");

        return Ok(new
        {
            userName = user.UserName,
            email = user.Email ?? "",
            emailConfirmed = user.EmailConfirmed,
            twoFactorEnabled = user.TwoFactorEnabled,
            lockoutEnabled = user.LockoutEnabled,
            lockoutEnd = user.LockoutEnd,
            lastLoginDate = user.LastLoginDate,
            accessFailedCount = user.AccessFailedCount
        });
    }
}
