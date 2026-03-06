using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionEcommerce.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Sửa từ Id thành OrderId để khớp Controller

        [Required]
        [StringLength(20)]
        public string OrderCode { get; set; } = string.Empty;

        public int? UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; } = string.Empty;

        // Tạo bí danh để Controller không báo lỗi 'CustomerName'
        [NotMapped]
        public string CustomerName { get => ShippingName; set => ShippingName = value; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string ShippingPhone { get; set; } = string.Empty;

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal? DiscountAmount { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public decimal? ShippingFee { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "COD";

        [StringLength(20)]
        public string? PaymentStatus { get; set; }

        public int? Status { get; set; }

        public virtual User? User { get; set; }
        
        // Dùng ICollection để linh hoạt hơn
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    }
}