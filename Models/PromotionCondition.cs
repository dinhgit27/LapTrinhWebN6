using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class PromotionCondition
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PromotionId { get; set; }

        [Required]
        [StringLength(50)]
        public string Field { get; set; }

        [Required]
        [StringLength(20)]
        public string Operator { get; set; }

        [Required]
        [StringLength(200)]
        public string Value { get; set; }

        // Navigation properties
        public virtual Promotion Promotion { get; set; }
    }
}