using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ColorId { get; set; }

        [Required]
        public int SizeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public decimal? PriceModifier { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual MasterColor Color { get; set; }
        public virtual MasterSize Size { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }
}