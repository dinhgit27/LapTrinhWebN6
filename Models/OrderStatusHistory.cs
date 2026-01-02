using System;
using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho lịch sử thay đổi trạng thái của đơn hàng
    // Theo dõi các thay đổi trạng thái theo thời gian với ghi chú và người thực hiện
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int OrderId { get; set; }  // Khóa ngoại đến Order

        public int? PreviousStatus { get; set; }  // Trạng thái trước khi thay đổi

        [Required]
        public int NewStatus { get; set; }  // Trạng thái mới

        [StringLength(255)]
        public string Note { get; set; }  // Ghi chú về thay đổi

        [StringLength(100)]
        public string UpdatedBy { get; set; }  // Người thực hiện thay đổi

        public DateTime? Timestamp { get; set; }  // Thời gian thay đổi

        // Navigation properties
        public virtual Order Order { get; set; }  // Đơn hàng được theo dõi
    }
}