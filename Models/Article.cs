using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho bài viết/blog trong hệ thống
    // Có thể là tin tức, hướng dẫn, bài viết marketing
    public class Article
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề bài viết

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }  // Slug URL-friendly duy nhất

        [StringLength(500)]
        public string Summary { get; set; }  // Tóm tắt ngắn

        public string Content { get; set; }  // Nội dung đầy đủ

        [StringLength(500)]
        public string Thumbnail { get; set; }  // URL ảnh đại diện

        [Required]
        public int CategoryId { get; set; }  // Khóa ngoại đến ArticleCategory

        public bool? IsPublished { get; set; }  // Bài viết đã xuất bản chưa

        public DateTime? PublishedAt { get; set; }  // Thời gian xuất bản

        // Navigation properties
        public virtual ArticleCategory Category { get; set; }  // Danh mục bài viết
    }
}