using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uranus.AuthApi.Models
{
    public class AvailableClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Type { get; set; }

        [Required]
        [MaxLength(256)]
        public string Value { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public int? ParentClaimId { get; set; } //Menus para claims hierárquicas

        [ForeignKey("ParentClaimId")]
        public AvailableClaim ParentClaim { get; set; } //Navegação para claim pai
    }
}