namespace Uranus.AuthApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task<bool> EnviarNotificacaoBloqueioAsync(string username, string email, DateTime? bloqueioAte);
    }
}
