using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    public class ArticleCategoryUpdateDto
    {
        //cap nhat danh muc
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Slug { get; set; }
    }
}