using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        public string Content { get; set; }

        [StringLength(500)]
        public string Thumbnail { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool? IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        public virtual ArticleCategory Category { get; set; }
    }
}