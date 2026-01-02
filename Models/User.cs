using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string GoogleId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [StringLength(500)]
        public string AvatarUrl { get; set; }

        [StringLength(20)]
        public string Role { get; set; }

        public bool? IsLocked { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new HashSet<UserAddress>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new HashSet<ProductReview>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<Coupon> Coupons { get; set; } = new HashSet<Coupon>();
    }
}