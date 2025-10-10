using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeShop.Repository.Entities
{
	public class Shoe
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string Brand { get; set; } = string.Empty;

		[Column(TypeName = "decimal(18,2)")]
		public decimal Cost { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Price { get; set; }

		[MaxLength(1000)]
		public string? Description { get; set; }

		[MaxLength(500)]
		public string? ImageUrl { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedDate { get; set; } = DateTime.Now;
		
		public virtual ICollection<ShoeColorVariation> ColorVariations { get; set; } = new List<ShoeColorVariation>();
	}
}