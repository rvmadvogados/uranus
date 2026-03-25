namespace Uranus.AuthApi.DTOs
{
    public class RoleDto
    {
        public string Role { get; set; }
        public List<string> Claims { get; set; }
    }
}