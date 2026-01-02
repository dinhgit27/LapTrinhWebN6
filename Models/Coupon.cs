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
        [StringLength(20)]
        public string DiscountType { get; set; } // e.g., "Percent", "Fixed"

        [Required]
        public decimal DiscountValue { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool? IsActive { get; set; } = true;

        public int? UsageLimit { get; set; }

        public int UsageCount { get; set; } = 0;

        // No direct navigation properties defined
    }
}