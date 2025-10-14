using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShoeShop.Repository.Entities.Enums;

namespace ShoeShop.Repository.Entities
{
	public class PurchaseOrder
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(50)]
		public string OrderNumber { get; set; } = string.Empty;

		[Required]
		public int SupplierId { get; set; }

		public DateTime OrderDate { get; set; } = DateTime.Now;

		public DateTime ExpectedDate { get; set; }

		public OrderStatus Status { get; set; } = OrderStatus.Pending;

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalAmount { get; set; }
		[ForeignKey(nameof(SupplierId))]
		public virtual Supplier Supplier { get; set; } = null!;
		public virtual ICollection<PurchaseOrderItem> OrderItems { get; set; } = new List<PurchaseOrderItem>();
	}
}
