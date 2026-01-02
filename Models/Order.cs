using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho đơn hàng
    // Chứa thông tin tổng quan về đơn hàng, khách hàng, địa chỉ giao hàng và trạng thái
    public class Order
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(20)]
        public string OrderCode { get; set; }  // Mã đơn hàng duy nhất

        public int? UserId { get; set; }  // Khóa ngoại đến User (có thể null cho khách vãng lai)

        public DateTime? OrderDate { get; set; }  // Ngày đặt hàng

        [Required]
        [StringLength(100)]
        public string ShippingName { get; set; }  // Tên người nhận

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; }  // Địa chỉ giao hàng

        [Required]
        [StringLength(15)]
        public string ShippingPhone { get; set; }  // Số điện thoại giao hàng

        [Required]
        public decimal TotalAmount { get; set; }  // Tổng tiền trước giảm giá

        public decimal? DiscountAmount { get; set; }  // Số tiền giảm giá

        [StringLength(50)]
        public string CouponCode { get; set; }  // Mã giảm giá áp dụng

        public decimal? ShippingFee { get; set; }  // Phí vận chuyển

        [Required]
        public decimal FinalAmount { get; set; }  // Tổng tiền cuối cùng

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }  // Phương thức thanh toán

        [StringLength(20)]
        public string PaymentStatus { get; set; }  // Trạng thái thanh toán

        public int? Status { get; set; }  // Trạng thái đơn hàng (0: Pending, 1: Confirmed, etc.)

        // Navigation properties
        public virtual User User { get; set; }  // Khách hàng đặt hàng
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();  // Chi tiết sản phẩm trong đơn
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();  // Lịch sử thay đổi trạng thái
    }
}