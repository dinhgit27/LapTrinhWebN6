using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model liên kết giữa sản phẩm và chương trình khuyến mãi
    // Xác định sản phẩm nào được áp dụng khuyến mãi nào
    public class ProductPromotion
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int ProductId { get; set; }  // Khóa ngoại đến Product

        [Required]
        public int PromotionId { get; set; }  // Khóa ngoại đến Promotion

        // Navigation properties
        public virtual Product Product { get; set; }  // Sản phẩm được khuyến mãi
        public virtual Promotion Promotion { get; set; }  // Chương trình khuyến mãi
    }
}