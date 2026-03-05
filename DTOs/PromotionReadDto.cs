using System.ComponentModel.DataAnnotations;
using FashionEcommerce.Models;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để trả về thông tin chương trình khuyến mãi
    /// </summary>
    public class PromotionReadDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool? IsActive { get; set; }

        public int? Priority { get; set; }

        public bool IsValid { get; set; }

        public string? Description { get; set; }

        public List<PromotionConditionReadDto>? PromotionConditions { get; set; }
    }

    /// <summary>
    /// DTO để trả về điều kiện khuyến mãi
    /// </summary>
    public class PromotionConditionReadDto
    {
        public int Id { get; set; }

        public string? Field { get; set; }

        public string? Operator { get; set; }

        public string? Value { get; set; }
    }

    /// <summary>
    /// DTO để tạo mới chương trình khuyến mãi
    /// </summary>
    public class PromotionCreateDto
    {
        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên khuyến mãi phải từ 3-200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [RegularExpression("FIXED_AMOUNT|PERCENTAGE", ErrorMessage = "Loại giảm giá phải là FIXED_AMOUNT hoặc PERCENTAGE")]
        public string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải là số dương")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, 100, ErrorMessage = "Độ ưu tiên phải từ 1-100")]
        public int? Priority { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }

        public List<PromotionConditionCreateDto>? PromotionConditions { get; set; }

        public List<int>? ProductIds { get; set; }
    }

    /// <summary>
    /// DTO để tạo điều kiện khuyến mãi
    /// </summary>
    public class PromotionConditionCreateDto
    {
        [Required(ErrorMessage = "Tên trường là bắt buộc")]
        [StringLength(50)]
        public string Field { get; set; } = string.Empty;

        [Required(ErrorMessage = "Toán tử so sánh là bắt buộc")]
        [StringLength(20)]
        [RegularExpression(@">=|<=|>|<|=|!=", ErrorMessage = "Toán tử phải là >=, <=, >, <, =, hoặc !=")]
        public string Operator { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá trị là bắt buộc")]
        [StringLength(200)]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO để cập nhật chương trình khuyến mãi
    /// </summary>
    public class PromotionUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên khuyến mãi phải từ 3-200 ký tự")]
        public string? Name { get; set; }

        [RegularExpression("FIXED_AMOUNT|PERCENTAGE", ErrorMessage = "Loại giảm giá phải là FIXED_AMOUNT hoặc PERCENTAGE")]
        public string? DiscountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải là số dương")]
        public decimal? DiscountValue { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; }

        [Range(1, 100, ErrorMessage = "Độ ưu tiên phải từ 1-100")]
        public int? Priority { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }
    }
}
