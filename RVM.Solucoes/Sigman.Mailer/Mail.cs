using System;
using System.Net;
using System.Net.Mail;

namespace Sigman.Mailer
{
    public static class Mail
    {
        public static string Send(string de, string para, string assunto, string corpo, string SMTPHost, string SMTPUsuario, string SMTPSenha, int SMTPPorta, string Arquivo)
        {
            try
            {
                // Monta as Credenciais
                NetworkCredential credential = new NetworkCredential();
                credential.UserName = SMTPUsuario;
                credential.Password = SMTPSenha;
                //credential.Domain = "maxiservcobranca.com.br";

                // Cria o Cliente SMTP:
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SMTPHost;
                smtp.Port = SMTPPorta;
                smtp.Credentials = credential;
                smtp.EnableSsl = false;

                // Monta Mensagem:

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;
                message.Body = corpo;
                message.Subject = assunto;

                if (Arquivo != null)
                {
                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(Arquivo);
                    message.Attachments.Add(attachment);
                }

                MailAddressCollection mailTo = new MailAddressCollection();
                foreach (var address in para.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(address);
                }

                // message.ReplyTo = new MailAddress(from);
                message.From = new MailAddress(de);

                // Envia Mensagem:
                smtp.Send(message);
                return "Sucesso";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}