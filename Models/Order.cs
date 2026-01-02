using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderCode { get; set; }

        public int? UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; }

        [Required]
        [StringLength(15)]
        public string ShippingPhone { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        [StringLength(50)]
        public string CouponCode { get; set; }

        public decimal? ShippingFee { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; }

        public int? Status { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();
    }
}