using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho sản phẩm trong cửa hàng
    // Chứa thông tin cơ bản về sản phẩm, giá cả, danh mục và các mối quan hệ với biến thể, hình ảnh, đánh giá
    public class Product
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(200)]
        public string Name { get; set; }  // Tên sản phẩm

        [Required]
        [StringLength(200)]
        public string Slug { get; set; }  // Slug URL-friendly duy nhất

        public string? Description { get; set; }  // Mô tả chi tiết sản phẩm

        [Required]
        public decimal Price { get; set; }  // Giá cơ bản

        [Required]
        public int CategoryId { get; set; }  // Khóa ngoại đến Category

        [StringLength(500)]
        public string? Thumbnail { get; set; }  // URL ảnh đại diện

        public bool? IsActive { get; set; }  // Sản phẩm có đang bán không

        // Navigation properties
        public virtual Category? Category { get; set; }  // Danh mục sản phẩm
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();  // Các biến thể (màu, size)
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new HashSet<ProductReview>();  // Đánh giá từ khách hàng
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();  // Hình ảnh bổ sung
        public virtual ICollection<ProductPromotion> ProductPromotions { get; set; } = new HashSet<ProductPromotion>();  // Khuyến mãi áp dụng
    }
}