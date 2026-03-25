using Uranus.AuthApi.Models;

namespace Uranus.AuthApi.Services
{
    public interface ITwoFactorService
    {
        Task SaveLastTwoFactorConfirmedAsync(ApplicationUser user);
        bool IsTwoFactorStillValid(ApplicationUser user, int validityMinutes = 15);
        Task ClearLastTwoFactorConfirmedAsync(ApplicationUser user);
    }
}
