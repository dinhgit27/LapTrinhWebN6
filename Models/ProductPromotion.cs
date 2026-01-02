using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class ProductPromotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int PromotionId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}