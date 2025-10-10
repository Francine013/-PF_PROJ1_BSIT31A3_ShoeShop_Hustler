using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Entities
{
    public class ShoeColorVariation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShoeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ColorName { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? HexCode { get; set; }

        public int StockQuantity { get; set; } = 0;

        public int ReorderLevel { get; set; } = 5;

        public bool IsActive { get; set; } = true;

        [ForeignKey("ShoeId")]
        public virtual Shoe Shoe { get; set; } = null!;

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public virtual ICollection<StockPullOut> StockPullOuts { get; set; } = new List<StockPullOut>();
    }
}