using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho danh mục của bài viết
    // Phân loại bài viết thành các chủ đề khác nhau
    public class ArticleCategory
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Tên danh mục

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }  // Slug URL-friendly duy nhất

        // Navigation properties
        public virtual ICollection<Article> Articles { get; set; } = new HashSet<Article>();  // Các bài viết trong danh mục
    }
}