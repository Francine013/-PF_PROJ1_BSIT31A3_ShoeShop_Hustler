using System.ComponentModel.DataAnnotations;

namespace ShoeShop.Services.DTOs
{
    public class CreatePullOutDto
    {
        [Required]
        public int ShoeColorVariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, StringLength(100)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ReasonDetails { get; set; }

        [Required, StringLength(200)]
        public string RequestedBy { get; set; } = string.Empty;
    }

    public class PullOutRequestDto
    {
        public int Id { get; set; }
        public int ShoeColorVariationId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetails { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
