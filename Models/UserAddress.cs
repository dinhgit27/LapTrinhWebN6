using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.Models
{
    // Model đại diện cho địa chỉ giao hàng của người dùng
    // Lưu trữ thông tin liên hệ và địa chỉ chi tiết cho việc giao hàng
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }  // Khóa chính

        [Required]
        public int UserId { get; set; }  // Khóa ngoại đến User

        [Required]
        [StringLength(100)]
        public string ContactName { get; set; }  // Tên người liên hệ

        [Required]
        [StringLength(15)]
        public string ContactPhone { get; set; }  // Số điện thoại liên hệ

        [Required]
        [StringLength(255)]
        public string AddressLine { get; set; }  // Địa chỉ chi tiết

        [Required]
        [StringLength(50)]
        public string Province { get; set; }  // Tỉnh/Thành phố

        [Required]
        [StringLength(50)]
        public string District { get; set; }  // Quận/Huyện

        [Required]
        [StringLength(50)]
        public string Ward { get; set; }  // Phường/Xã

        public bool? IsDefault { get; set; }  // Địa chỉ mặc định hay không

        // Navigation properties
        public virtual User User { get; set; }  // Tham chiếu đến User sở hữu địa chỉ này
    }
}