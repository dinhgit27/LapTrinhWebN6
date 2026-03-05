using System;
using System.Collections.Generic;
using FashionEcommerce.DTOs;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để trả về thông tin sản phẩm
    /// </summary>
    public class ProductReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? Thumbnail { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public ICollection<ProductVariantReadDto>? ProductVariants { get; set; }
    }
}
