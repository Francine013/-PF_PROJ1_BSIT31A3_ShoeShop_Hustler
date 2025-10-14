using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoeShop.Repository.Entities.Enums;

namespace ShoeShop.Repository.Entities
{
    public class StockPullOut
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShoeColorVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ReasonDetails { get; set; }

        [Required]
        [MaxLength(200)]
        public string RequestedBy { get; set; } = string.Empty; 

        [MaxLength(200)]
        public string? ApprovedBy { get; set; } 

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2")]
        public DateTime? ApprovedDate { get; set; }

        public PullOutStatus Status { get; set; } = PullOutStatus.Pending;

        [ForeignKey(nameof(ShoeColorVariationId))]
        public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
    }
}
