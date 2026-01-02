using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductVariantId { get; set; }

        [Required]
        [StringLength(200)]
        public string Snapshot_ProductName { get; set; }

        [Required]
        [StringLength(50)]
        public string Snapshot_Sku { get; set; }

        [StringLength(500)]
        public string Snapshot_Thumbnail { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}