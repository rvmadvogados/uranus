using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Text;
using Uranus.AuthApi.Models;
using Uranus.AuthApi.Services;

namespace Uranus.AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TwoFactorController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public TwoFactorController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("enable")]
        [Authorize]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(authenticatorKey))
            {
                return BadRequest(new { message = "Erro ao gerar chave do autenticador" });
            }

            var formattedKey = FormatKey(authenticatorKey);

            var appName = _configuration["TwoFactor:AppName"] ?? "Uranus";
            var qrCodeUri = GenerateQrCodeUri(user.Email ?? user.UserName, authenticatorKey, appName);

            var qrCodeBase64 = GenerateQrCode(qrCodeUri);

            return Ok(new EnableTwoFactorResponse
            {
                QrCodeUri = qrCodeBase64,
                ManualEntryKey = formattedKey,
                Message = "Escaneie o QR Code com o Google Authenticator ou insira a chave manualmente"
            });
        }

        [HttpPost("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
            {
                return BadRequest(new { message = "Código inválido" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            
            return Ok(new
            {
                message = "Autenticação de dois fatores ativada com sucesso",
                recoveryCodes = recoveryCodes?.ToList()
            });
        }

        [HttpPost("disable")]
        [Authorize]
        public async Task<IActionResult> DisableTwoFactor([FromBody] DisableTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
            {
                return BadRequest(new { message = "Código inválido" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            return Ok(new { message = "Autenticação de dois fatores desativada com sucesso" });
        }

        
        [HttpPost("admin/reset")]
        public async Task<IActionResult> AdminResetTwoFactor([FromBody] AdminResetTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var wasTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var hadAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));

            await _userManager.SetTwoFactorEnabledAsync(user, false);

            await _userManager.ResetAuthenticatorKeyAsync(user);

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 0);

            user.RequiresTwoFactorSetup = true;
            await _userManager.UpdateAsync(user);
            var newKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (!string.IsNullOrEmpty(newKey))
            {
                newKey = newKey.ToUpper();
            }

            if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(newKey))
            {
                try
                {
                    var appName = _configuration["TwoFactor:AppName"] ?? "Uranus";
                    var qrCodeUri = GenerateQrCodeUri(user.Email ?? user.UserName, newKey, appName);
                    var qrCodeBase64 = GenerateQrCode(qrCodeUri);

                    var subject = "Reconfiguração de Autenticação 2FA - Uranus";

                    var htmlContent = GerarEmailRecuperacao2FA(user.UserName, newKey, qrCodeBase64);

                    await _emailService.SendEmailAsync(user.Email, subject, htmlContent);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }


            return Ok(new
            {
                message = $"2FA completamente resetado para o usuário {user.UserName}. Email enviado.",
                username = user.UserName,
                wasTwoFactorEnabled = wasTwoFactorEnabled,
                hadAuthenticator = hadAuthenticator,
                details = new
                {
                    twoFactorDisabled = true,
                    authenticatorKeyReset = true,
                    recoveryCodesInvalidated = true,
                    requiresNewSetup = true,
                    emailSent = !string.IsNullOrEmpty(user.Email)
                }
            });
        }

        
        [HttpPost("admin/enable")]
        //[Authorize(Roles = "Admin,Administrador")]
        public async Task<IActionResult> AdminEnableTwoFactor([FromBody] AdminResetTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var wasEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            string message;
            if (hasAuthenticator)
            {
                message = $"2FA reativado para {user.UserName}. Dispositivo anterior será usado.";
            }
            else
            {
                message = $"2FA ativado para {user.UserName}. Usuário será obrigado a configurar no próximo login.";
            }

            return Ok(new
            {
                message = message,
                username = user.UserName,
                wasEnabled = wasEnabled,
                hasAuthenticator = hasAuthenticator,
                twoFactorEnabled = true,
                requiresSetup = !hasAuthenticator, // Se não tem authenticator, precisa configurar
                details = new
                {
                    action = "enable",
                    authenticatorPreserved = hasAuthenticator,
                    recoveryCodesPreserved = true
                }
            });
        }

        [HttpPost("admin/disable")]
        public async Task<IActionResult> AdminDisableTwoFactor([FromBody] AdminResetTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var wasEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));
            var recoveryCodesCount = await _userManager.CountRecoveryCodesAsync(user);

            if (!wasEnabled)
            {
                return BadRequest(new { message = "2FA já está desativado para este usuário" });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);

            return Ok(new
            {
                message = $"2FA desativado para {user.UserName}. Configuração preservada (pode reativar sem reconfigurar).",
                username = user.UserName,
                wasEnabled = wasEnabled,
                twoFactorEnabled = false,
                details = new
                {
                    action = "disable",
                    authenticatorPreserved = hasAuthenticator,
                    recoveryCodesPreserved = recoveryCodesCount > 0,
                    recoveryCodesCount = recoveryCodesCount,
                    canReactivate = hasAuthenticator // Se tem authenticator, pode reativar
                }
            });
        }

        [HttpGet("status/{username}")]
        [Authorize]
        public async Task<IActionResult> GetTwoFactorStatus(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));

            return Ok(new
            {
                username = user.UserName,
                twoFactorEnabled = isTwoFactorEnabled,
                hasAuthenticator = hasAuthenticator,
                recoveryCodesRemaining = await _userManager.CountRecoveryCodesAsync(user)
            });
        }

        [HttpPost("recovery-codes")]
        [Authorize]
        public async Task<IActionResult> GenerateRecoveryCodes([FromBody] VerifyTwoFactorRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                return BadRequest(new { message = "2FA não está habilitado para este usuário" });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code);

            if (!isValid)
            {
                return BadRequest(new { message = "Código inválido" });
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

            return Ok(new
            {
                message = "Novos códigos de recuperação gerados",
                recoveryCodes = recoveryCodes?.ToList()
            });
        }

        [HttpPost("send-email-code")]
        public async Task<IActionResult> SendEmailCode([FromBody] SendEmailCodeRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { message = "Usuário não possui email configurado" });
            }

            var code = new Random().Next(100000, 999999).ToString();

            
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

            // Enviar email
            var subject = "Código de Autenticação - Uranus";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                        <h2 style='color: #365873;'>Código de Autenticação</h2>
                        <p>Olá,</p>
                        <p>Você solicitou um código de autenticação para acessar o sistema Uranus.</p>
                        <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0; text-align: center; border-radius: 5px;'>
                            <h1 style='color: #365873; letter-spacing: 5px; margin: 0;'>{token}</h1>
                        </div>
                        <p><strong>Este código é válido por 10 minutos.</strong></p>
                        <p>Se você não solicitou este código, por favor ignore este email.</p>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                        <p style='color: #666; font-size: 12px;'>
                            Esta é uma mensagem automática. Por favor, não responda este email.
                        </p>
                    </div>
                </body>
                </html>
            ";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok(new
            {
                message = "Código enviado para o email cadastrado",
                email = MaskEmail(user.Email)
            });
        }

        [HttpPost("verify-email-code")]
        public async Task<IActionResult> VerifyEmailCode([FromBody] VerifyEmailCodeRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultEmailProvider,
                request.Code);

            if (!isValid)
            {
                return BadRequest(new { message = "Código inválido ou expirado" });
            }

            return Ok(new { message = "Código verificado com sucesso" });
        }

        [HttpPost("set-preferred-method")]
        [Authorize]
        public async Task<IActionResult> SetPreferredMethod([FromBody] SetPreferredMethodRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            user.PreferredTwoFactorMethod = request.Method; // retirar
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                message = $"Método preferencial configurado: {request.Method}",
                method = request.Method
            });
        }

        [HttpGet("preferred-method/{username}")]
        public async Task<IActionResult> GetPreferredMethod(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            var hasAuthenticator = !string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user));
            var hasEmail = !string.IsNullOrEmpty(user.Email);

            return Ok(new
            {
                preferredMethod = user.PreferredTwoFactorMethod ?? (hasAuthenticator ? "App" : "Email"),
                hasAuthenticator = hasAuthenticator,
                hasEmail = hasEmail,
                email = hasEmail ? MaskEmail(user.Email) : null
            });
        }

        private string GenerateQrCodeUri(string email, string authenticatorKey, string appName)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                Uri.EscapeDataString(appName),
                Uri.EscapeDataString(email),
                authenticatorKey);
        }

        private string GenerateQrCode(string uri)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
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

        
        private string GerarEmailRecuperacao2FA(string username, string chave, string qrCodeBase64)
        {
            if (string.IsNullOrEmpty(qrCodeBase64))
            {
                System.Diagnostics.Debug.WriteLine("[GerarEmailRecuperacao2FA] AVISO: QR Code está vazio!");
            }

            return $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ 
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
     color: #333; 
         line-height: 1.6;
          background-color: #f5f5f5;
        }}
     .container {{ 
            max-width: 600px; 
    margin: 0 auto; 
      background: white; 
         box-shadow: 0 2px 10px rgba(0,0,0,0.1); 
        border-radius: 8px;
            overflow: hidden;
  }}
  .header {{ 
            background: linear-gradient(135deg, #F39C12 0%, #E67E22 100%); 
            color: white; 
        padding: 40px; 
            text-align: center;
        }}
        .header h1 {{ 
            margin: 0; 
    font-size: 28px;
      font-weight: 600;
        }}
        .header p {{ 
            margin: 10px 0 0 0; 
            opacity: 0.95;
   font-size: 14px;
    }}
        .body {{ 
            padding: 40px; 
        }}
        .greeting {{
         font-size: 16px;
 margin-bottom: 20px;
            color: #333;
 }}
        .greeting strong {{
    color: #F39C12;
        }}
        .message {{
       font-size: 15px;
          margin-bottom: 30px;
     line-height: 1.8;
       color: #555;
        }}
  .warning {{ 
            background: #FCF8E3; 
  border-left: 4px solid #F39C12; 
            padding: 15px; 
        border-radius: 4px; 
            margin: 20px 0;
        }}
        .warning strong {{ 
     color: #856404;
            display: block;
         margin-bottom: 5px;
        }}
        .warning p {{ 
       color: #856404; 
 font-size: 14px; 
       margin: 0;
 }}
    .qr-section {{
        background: #FFF9E6;
       border: 2px solid #F39C12;
            padding: 20px;
       border-radius: 4px;
     margin: 25px 0;
            text-align: center;
        }}
.qr-section h3 {{
  color: #E67E22;
            font-size: 16px;
          margin-bottom: 15px;
            font-weight: 600;
        }}
        .qr-code {{
 background: white;
  padding: 15px;
       border: 1px solid #E0E0E0;
            border-radius: 4px;
  display: inline-block;
      margin: 0 auto;
     }}
        .qr-instruction {{
            font-size: 12px;
 color: #856404;
    margin-top: 10px;
  font-weight: 500;
        }}
        .key-box {{ 
  background: #FFF3E0; 
            border-left: 4px solid #F39C12; 
       padding: 20px; 
         border-radius: 4px; 
            margin: 25px 0;
        }}
 .key-label {{ 
        font-size: 12px; 
        color: #E65100; 
     text-transform: uppercase; 
          letter-spacing: 1px; 
            margin-bottom: 12px; 
            font-weight: 600;
     }}
        .key-value {{ 
            font-family: 'Courier New', monospace; 
 font-size: 18px; 
        font-weight: bold; 
       color: #E65100; 
   letter-spacing: 2px; 
    text-align: center; 
            background: white; 
   padding: 12px; 
            border-radius: 4px;
       word-break: break-all;
line-height: 1.4;
        }}
        .instructions {{ 
            background: #F9F9F9; 
          padding: 20px; 
            border-radius: 4px; 
      margin: 25px 0;
       border: 1px solid #E0E0E0;
        }}
  .instructions h3 {{ 
 color: #F39C12; 
            font-size: 16px; 
      margin-bottom: 12px;
            font-weight: 600;
      }}
        .instructions ol {{ 
            margin-left: 20px; 
  }}
     .instructions li {{ 
     margin-bottom: 10px; 
       font-size: 14px;
            color: #555;
        }}
        .divider {{
            height: 1px;
       background: #E0E0E0;
       margin: 30px 0;
      }}
      .info-box {{
       background: #F9F9F9;
  padding: 20px;
          border-radius: 4px;
            border: 1px solid #E0E0E0;
       margin: 20px 0;
    }}
    .info-box strong {{
   color: #F39C12;
            display: block;
       margin-bottom: 10px;
     }}
        .info-box ul {{
    margin-left: 20px;
         color: #555;
            font-size: 14px;
        }}
        .info-box li {{
          margin-bottom: 8px;
     }}
    .footer {{ 
            background: #F9F9F9; 
 padding: 30px; 
            text-align: center; 
        border-top: 1px solid #E0E0E0; 
    font-size: 12px; 
   color: #999;
        }}
    .footer p {{
    margin-bottom: 8px;
        }}
    .company-info {{
   color: #999;
 font-size: 11px;
 margin-top: 15px;
        }}
        .company-info strong {{
            color: #F39C12;
    }}
    </style>
</head>
<body>
    <div class='container'>
   <!-- Header -->
  <div class='header'>
        <h1>Reconfiguração de chave Google Authenticator</h1>
            <p>Autenticação de Dois Fatores - Uranus</p>
        </div>

     <!-- Body -->
        <div class='body'>
          <div class='greeting'>
                Olá <strong>{username}</strong>,
            </div>

            <div class='message'>
             Sua autenticação de dois fatores (2FA) foi resetada por um administrador do sistema. 
      Para continuar acessando sua conta, você precisará reconfigurar seu Google Authenticator 
            usando a chave fornecida abaixo.
 </div>


            <!-- QR Code Section -->
     <div class='qr-section'>
<h3>Configuração Rápida</h3>
    <div class='qr-code'>
  <p style='font-size: 14px; color: #555; margin: 0;'>Use a chave de configuração manual abaixo para adicionar sua conta ao Google Authenticator.</p>
      </div>
       <div class='qr-instruction'>
      Abra o Google Authenticator e insira a chave manualmente
        </div>
   </div>

         <!-- Key Section for Manual Entry -->
       <div class='key-box'>
    <div class='key-label'>Chave de Configuração Manual</div>
<div class='key-value'>{chave}</div>
     </div>

            <!-- Instructions -->
  <div class='instructions'>
       <h3>Como Reconfigurar seu 2FA:</h3>
 <ol>
      <li><strong>Abra o Google Authenticator no seu celular</strong></li>
      <li><strong>Clique no '+' para adicionar uma conta</strong></li>
      <li><strong>Selecione 'Inserir uma chave de configuração'</strong></li>
      <li><strong>Insira a chave manual exibida acima:</strong>
   <ul style='margin: 5px 0 0 20px;'>
        <li style='font-family: monospace; font-weight: bold; color: #E65100;'>{chave}</li>
        </ul>
      </li>
      <li><strong>Confirme a adição e anote os códigos de recuperação</strong></li>
  <li><strong>Faça login e será solicitado seu novo código 2FA</strong></li>
     </ol>
   </div>

 <div class='divider'></div>

            <!-- Info Box -->
            <div class='info-box'>
         <strong>Informações Importantes:</strong>
                <ul>
      <li>A chave é pessoal e intransferível</li>
   <li>Você será solicitado a digitar o código 2FA no próximo login</li>
         <li>O 2FA será obrigatório para acessar sua conta</li>
    </ul>
         </div>

   <div class='divider'></div>

          <div class='message' style='font-size: 13px; color: #999;'>
    Se tiver dúvidas, entre em contato com o suporte técnico ou com um administrador do sistema.
       </div>
        </div>

        <!-- Footer -->
        <div class='footer'>
        <p>Este é um email automático gerado pelo sistema de autenticação.</p>
     <p>Por favor, não responda este email.</p>
  </div>
    </div>
</body>
</html>";
        }

        [HttpPost("skip-authenticator-setup")]
        [Authorize]
        public async Task<IActionResult> SkipAuthenticatorSetup([FromBody] SkipAuthenticatorSetupRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

     
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                System.Diagnostics.Debug.WriteLine($"[2FA SKIP] Chave authenticator gerada: {authenticatorKey}");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            user.RequiresTwoFactorSetup = false;
            await _userManager.UpdateAsync(user);


            return Ok(new
            {
                message = "Configuração concluída. Você usará 2FA via Email.",
                username = user.UserName,
                twoFactorEnabled = true,
                hasAuthenticator = true,
                usesEmailOnly = true
            });
        }
    }
}
