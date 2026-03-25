using System.ComponentModel.DataAnnotations;

namespace Uranus.AuthApi.Models
{
    public class MigrationUserModel
    {
        [Required]
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        [Required]
        public string LegacyMd5Hash { get; set; } = String.Empty;
        public string? Role { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class BatchMigrationModel
    {
        public List<MigrationUserModel> Users { get; set; } = new List<MigrationUserModel>();
    }
}