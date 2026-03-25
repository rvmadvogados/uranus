using Microsoft.AspNetCore.Identity;

namespace Uranus.AuthApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string LegacyMd5Hash { get; set; } = String.Empty;
        public string PreferredTwoFactorMethod { get; set; } = "App"; // "App" ou "Email"
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastFailedLoginDate { get; set; } 
        public DateTime? LastTwoFactorConfirmed { get; set; } 
        public bool RequiresTwoFactorSetup { get; set; } = false;
    }
}