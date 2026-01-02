using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public int? Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        public virtual Order Order { get; set; }
    }
}