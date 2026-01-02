using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        public int? SortOrder { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
    }
}