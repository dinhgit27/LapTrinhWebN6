using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho người dùng trong hệ thống
    // Chứa thông tin cá nhân, xác thực và các mối quan hệ với đơn hàng, địa chỉ, giỏ hàng, đánh giá, thông báo
    public class User
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [StringLength(50)]
        public string? Username { get; set; }  // Tên đăng nhập (có thể null)

        [StringLength(255)]
        public string? PasswordHash { get; set; }  // Mật khẩu đã hash (có thể null cho đăng nhập Google)

        [Required]
        [StringLength(100)]
        public string Email { get; set; }  // Email duy nhất, bắt buộc

        [StringLength(100)]
        public string? GoogleId { get; set; }  // ID Google cho đăng nhập OAuth

        public DateTime? DateOfBirth { get; set; }  // Ngày sinh

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }  // Họ và tên đầy đủ

        [StringLength(15)]
        public string? PhoneNumber { get; set; }  // Số điện thoại

        [StringLength(500)]
        public string? AvatarUrl { get; set; }  // URL ảnh đại diện

        [StringLength(20)]
        public string? Role { get; set; }  // Vai trò (Customer, Admin, etc.)

        public bool? IsLocked { get; set; }  // Tài khoản có bị khóa không

        public DateTime? CreatedAt { get; set; }  // Thời gian tạo tài khoản

        // Navigation properties - Các mối quan hệ
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new HashSet<UserAddress>();  // Địa chỉ giao hàng
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();  // Sản phẩm trong giỏ hàng
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();  // Đơn hàng
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new HashSet<ProductReview>();  // Đánh giá sản phẩm
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();  // Thông báo
        public virtual ICollection<Coupon> Coupons { get; set; } = new HashSet<Coupon>();  // Mã giảm giá
    }
}