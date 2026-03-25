using System.ComponentModel.DataAnnotations;

namespace Uranus.AuthApi.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginWith2FARequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 6)]
        public string TwoFactorCode { get; set; }
    }
}
