using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho chương trình khuyến mãi
    // Xác định các ưu đãi giảm giá áp dụng cho sản phẩm hoặc đơn hàng
    public class Promotion
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(200)]
        public string Name { get; set; }  // Tên chương trình khuyến mãi

        [Required]
        public DiscountType DiscountType { get; set; }  // Loại giảm giá (số tiền cố định hoặc phần trăm)

        [Required]
        public decimal DiscountValue { get; set; }  // Giá trị giảm giá

        [Required]
        public DateTime StartDate { get; set; }  // Ngày bắt đầu

        [Required]
        public DateTime EndDate { get; set; }  // Ngày kết thúc

        public bool? IsActive { get; set; }  // Chương trình có đang hoạt động không

        public int? Priority { get; set; }  // Độ ưu tiên khi áp dụng nhiều khuyến mãi

        // Navigation properties
        public virtual ICollection<PromotionCondition> PromotionConditions { get; set; } = new HashSet<PromotionCondition>();  // Điều kiện áp dụng
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new HashSet<ProductPromotion>();  // Sản phẩm áp dụng
        public virtual ICollection<Coupon> Coupons { get; set; } = new HashSet<Coupon>();  // Mã giảm giá liên quan
    }
}