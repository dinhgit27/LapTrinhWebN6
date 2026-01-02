using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string DiscountType { get; set; }

        [Required]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool? IsActive { get; set; }

        public int? Priority { get; set; }

        // Navigation properties
        public virtual ICollection<PromotionCondition> PromotionConditions { get; set; } = new HashSet<PromotionCondition>();
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new HashSet<ProductPromotion>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new HashSet<Coupon>();
    }
}