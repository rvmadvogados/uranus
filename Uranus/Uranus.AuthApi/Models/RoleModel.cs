using Uranus.AuthApi.Controllers;

namespace Uranus.AuthApi.Models
{
    public class RoleModel
    {
        public string Role { get; set; }
        public List<ClaimModel> Claims { get; set; }
    }
}
