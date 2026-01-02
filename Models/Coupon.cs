using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PromotionId { get; set; }

        public bool? IsUsed { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}