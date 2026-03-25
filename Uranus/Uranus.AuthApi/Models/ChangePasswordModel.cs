namespace Uranus.AuthApi.Models;

public class ChangePasswordModel
{
    public string UserName { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
