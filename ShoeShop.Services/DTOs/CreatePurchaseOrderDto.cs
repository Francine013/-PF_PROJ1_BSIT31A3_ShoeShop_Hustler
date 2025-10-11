using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 
using System; 

namespace ShoeShop.Services.DTOs
{
    public class PurchaseOrderItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; } 
        [Required(ErrorMessage = "Please select a Shoe Variation.")]
        [Display(Name = "Shoe Variation")]
        public int ShoeColorVariationId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000.")]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }

        public int QuantityReceived { get; set; }

        [Required(ErrorMessage = "Unit Cost is required.")]
        [Range(0.01, 10000.00, ErrorMessage = "Unit Cost must be a positive value.")]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }
    }

    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        public DateTime SoldQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        

        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderDto
    {
        [Required(ErrorMessage = "Order Number is required.")]
        [StringLength(50, ErrorMessage = "Order Number cannot exceed 50 characters.")]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a Supplier.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Order Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Expected Date")]
        public DateTime? ExpectedDate { get; set; }

        [Required(ErrorMessage = "The order must contain at least one item.")]
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}