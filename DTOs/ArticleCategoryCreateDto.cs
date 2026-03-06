using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    //tao danh muc
    public class ArticleCategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }
    }
}