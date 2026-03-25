using Microsoft.AspNetCore.Identity;

namespace Uranus.AuthApi.Models
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsActive { get; set; } = true;
    }
}