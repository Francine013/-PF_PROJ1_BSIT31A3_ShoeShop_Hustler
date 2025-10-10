using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Entities
{
    public class PurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public int ShoeColorVariationId { get; set; }

        public int QuantityOrdered { get; set; }

        public int QuantityReceived { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

        [ForeignKey("ShoeColorVariationId")]
        public virtual ShoeColorVariation ShoeColorVariation { get; set; } = null!;
    }
}