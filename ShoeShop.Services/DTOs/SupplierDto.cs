

using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? ContactPerson { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }
    }
}
