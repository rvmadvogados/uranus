using System.ComponentModel.DataAnnotations;

namespace Uranus.AuthApi.Models
{
    public class EnableTwoFactorRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class EnableTwoFactorResponse
    {
        public string QrCodeUri { get; set; }
        public string ManualEntryKey { get; set; }
        public string Message { get; set; }
    }

    public class VerifyTwoFactorRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 6)]
        public string Code { get; set; }
    }

    public class DisableTwoFactorRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 6)]
        public string Code { get; set; }
    }

    public class AdminResetTwoFactorRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class SendEmailCodeRequest
    {
        [Required]
        public string Username { get; set; }
    }

    public class VerifyEmailCodeRequest
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8, MinimumLength = 6)]
        public string Code { get; set; }
    }

    public class SetPreferredMethodRequest
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [RegularExpression("^(App|Email)$")]
        public string Method { get; set; } // "App" ou "Email"
    }

    public class TwoFactorSetupRequest
    {
        public string UserId { get; set; }
    }

    public class TwoFactorSetupResponse
    {
        public string QrCodeUrl { get; set; }
        public string SecretKey { get; set; }
    }

    public class TwoFactorVerifyRequest
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

    public class TwoFactorVerifyResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public long ConfirmedAt { get; set; }
    }

    public class TwoFactorCheckRequest
    {
        public string UserId { get; set; }
    }

    public class TwoFactorCheckResponse
    {
        public bool IsRequired { get; set; }
        public string Message { get; set; }
        public long LastConfirmedAt { get; set; }
    }

    public class TwoFactorDisableRequest
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

    public class TwoFactorDisableResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class SkipAuthenticatorSetupRequest
    {
        [Required]
        public string Username { get; set; }
    }
}
