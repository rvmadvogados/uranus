using System.Net;
using System.Net.Mail;

namespace Uranus.AuthApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
                var fromName = _configuration["Email:FromName"] ?? "Uranus - Sistema RVM";

                if (string.IsNullOrEmpty(smtpUser))
                {
                    _logger.LogError("Email:SmtpUser nao configurado no appsettings.json");
                    throw new InvalidOperationException("Configuracao de email incompleta: SmtpUser nao definido");
                }

                if (string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogError("Email:SmtpPassword nao configurado no appsettings.json");
                    throw new InvalidOperationException("Configuracao de email incompleta: SmtpPassword nao definido");
                }

                _logger.LogInformation($"Tentando enviar email para {to} via {smtpHost}:{smtpPort}");

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, fromName);
                    message.To.Add(new MailAddress(to));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(smtpHost, smtpPort))
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Timeout = 60000;

                        await client.SendMailAsync(message);
                        _logger.LogInformation($"Email enviado com sucesso para {to}");
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"Erro SMTP ao enviar email para {to}. StatusCode: {smtpEx.StatusCode}");

                if (smtpEx.Message.Contains("Authentication Required") || smtpEx.Message.Contains("5.7.0"))
                {
                    throw new InvalidOperationException(
                        "Falha na autenticacao com o servidor Gmail. " +
                        "Certifique-se de usar uma Senha de App do Google (nao a senha normal). " +
                        "Gere em: https://myaccount.google.com/apppasswords",
                        smtpEx);
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar email para {to}");
                throw;
            }
        }

        public async Task<bool> EnviarNotificacaoBloqueioAsync(string username, string email, DateTime? bloqueioAte)
        {
            try
            {
                var emailAdministrador = _configuration["Email:AdminBloqueioNotificacao"];

                var assunto = "Alerta de Seguranca: Usuario Bloqueado por Tentativas Falhas";
                var corpo = ConstruirCorpoEmailBloqueio(username, email, bloqueioAte);

                await SendEmailAsync(emailAdministrador, assunto, corpo);

                if (!string.IsNullOrEmpty(email))
                {
                    try
                    {
                        await SendEmailAsync(email, assunto, corpo);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Nao foi possivel enviar email de bloqueio para o usuario {email}: {ex.Message}");
                    }
                }
                _logger.LogInformation($"Notificacao de bloqueio enviada para admin e usuario: {username}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar notificacao de bloqueio para {username}: {ex.Message}");
                return false;
            }
        }

        private string ConstruirCorpoEmailBloqueio(string username, string email, DateTime? bloqueioAte)
        {
            var corpo = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <style>
                    body {{ font-family: Arial, sans-serif; color: #333; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #dc3545; color: white; padding: 20px; border-radius: 5px; margin-bottom: 20px; }}
                    .header h1 {{ margin: 0; font-size: 24px; }}
                    .content {{ background-color: #f9f9f9; padding: 20px; border-left: 4px solid #dc3545; margin-bottom: 20px; }}
                    .footer {{ color: #666; font-size: 12px; margin-top: 20px; border-top: 1px solid #ddd; padding-top: 20px; }}
                    .info-box {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin-bottom: 15px; }}
                    .danger-box {{ background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin-bottom: 15px; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    td {{ padding: 10px; border-bottom: 1px solid #ddd; }}
                    .label {{ font-weight: bold; width: 150px; background-color: #f0f0f0; }}
                    ul {{ margin: 10px 0; padding-left: 20px; }}
                    li {{ margin: 5px 0; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Alerta de Segurança</h1>
                        <p style='margin: 10px 0 0 0;'>Usuário Bloqueado por Tentativas Falhas de Login</p>
                    </div>

                    <div class='content'>
                        <h2>Detalhes do Bloqueio</h2>
                        <table>
                            <tr>
                                <td class='label'>Usuário:</td>
                                <td><strong>{username}</strong></td>
                            </tr>
                            <tr>
                                <td class='label'>Email:</td>
                                <td><strong>{(string.IsNullOrEmpty(email) ? "Não configurado" : email)}</strong></td>
                            </tr>
                            <tr>
                                <td class='label'>Data/Hora do Bloqueio:</td>
                                <td><strong>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</strong></td>
                            </tr>
                            <tr>
                                <td class='label'>Motivo:</td>
                                <td><strong>Excesso de tentativas de login com senha incorreta (5 tentativas em 5 minutos)</strong></td>
                            </tr>
                            <tr>
                                <td class='label'>Bloqueio Expira Em:</td>
                                <td><strong>{(bloqueioAte.HasValue ? bloqueioAte.Value.ToString("dd/MM/yyyy HH:mm:ss") : "Nao definido")}</strong></td>
                            </tr>
                        </table>
                    </div>

                    <div class='danger-box'>
                        <h3>Recomendações de Segurança</h3>
                        <ul>
                            <li><strong>Verifique se este login foi realizado por você</strong></li>
                            <li>Se não foi, sua senha pode ter sido comprometida</li>
                            <li>Altere sua senha assim que o bloqueio expirar</li>
                            <li>Use uma senha forte</li>
                            <li>Entre em contato com o administrador se precisar desbloquear imediatamente</li>
                        </ul>
                    </div>

                    <div class='info-box'>
                        <h3>O que fazer agora?</h3>
                        <p>Sua conta foi bloqueada temporariamente como medida de segurança. O bloqueio será removido automaticamente no horário descrito acima, ou você poderá contactar o administrador do sistema para desbloquear imediatamente.</p>
                    </div>

                    <div class='footer'>
                        <p><em>Este é um email automático gerado pelo sistema de autenticação. Por favor, não responda a este email.</em></p>
                        <p style='color: #999; font-size: 11px;'>Uranus RVM - Sistema de Autenticação</p>
                    </div>
                </div>
            </body>
            </html>";

            return corpo;
        }
    }
}
