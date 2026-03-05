using System.ComponentModel.DataAnnotations;

namespace FashionEcommerce.DTOs
{
    /// <summary>
    /// DTO để tính toán khuyến mãi cho một sản phẩm
    /// </summary>
    public class PromotionCalculationRequest
    {
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID sản phẩm phải là số dương")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải là số dương")]
        public int Quantity { get; set; }

        /// <summary>
        /// ID biến thể (nếu áp dụng cho biến thể cụ thể)
        /// </summary>
        public int? VariantId { get; set; }

        /// <summary>
        /// Mã coupon (nếu có)
        /// </summary>
        [StringLength(50)]
        public string? CouponCode { get; set; }
    }

    /// <summary>
    /// DTO để giỏ hàng tính toán khuyến mãi
    /// </summary>
    public class CartPromotionRequest
    {
        [Required(ErrorMessage = "Danh sách sản phẩm không được trống")]
        public List<PromotionCalculationRequest>? Items { get; set; }

        /// <summary>
        /// ID người dùng để áp dụng coupon
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "ID người dùng phải là số dương")]
        public int? UserId { get; set; }

        /// <summary>
        /// Mã coupon chung cho toàn giỏ
        /// </summary>
        [StringLength(50)]
        public string? CouponCode { get; set; }
    }

    /// <summary>
    /// DTO kết quả tính toán khuyến mãi cho một sản phẩm
    /// </summary>
    public class ProductDiscountDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal OriginalPrice { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Tổng tiền gốc (giá × số lượng)
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// Tiền khuyến mãi
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Phần trăm khuyến mãi
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Giá sau khuyến mãi
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Tổng tiền sau khuyến mãi (giá sau × số lượng)
        /// </summary>
        public decimal FinalTotal { get; set; }

        /// <summary>
        /// Chi tiết các khuyến mãi áp dụng
        /// </summary>
        public List<AppliedPromotionDto>? AppliedPromotions { get; set; }
    }

    /// <summary>
    /// DTO kết quả tính toán khuyến mãi cho giỏ hàng
    /// </summary>
    public class CartDiscountResultDto
    {
        /// <summary>
        /// Chi tiết discount từng sản phẩm
        /// </summary>
        public List<ProductDiscountDto>? Items { get; set; }

        /// <summary>
        /// Tổng tiền gốc
        /// </summary>
        public decimal SubtotalOriginal { get; set; }

        /// <summary>
        /// Tổng tiền khuyến mãi từ sản phẩm
        /// </summary>
        public decimal ProductDiscounts { get; set; }

        /// <summary>
        /// Discount từ coupon
        /// </summary>
        public decimal CouponDiscount { get; set; }

        /// <summary>
        /// Tổng tiền khuyến mãi
        /// </summary>
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Tổng tiền cuối cùng
        /// </summary>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// Phần trăm khuyến mãi chung
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Thông tin coupon được áp dụng
        /// </summary>
        public CouponAppliedDto? CouponApplied { get; set; }
    }

    /// <summary>
    /// DTO về khuyến mãi được áp dụng
    /// </summary>
    public class AppliedPromotionDto
    {
        public int PromotionId { get; set; }

        public string PromotionName { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }

        public decimal DiscountAmount { get; set; }

        public int Priority { get; set; }
    }

    /// <summary>
    /// DTO về coupon được áp dụng
    /// </summary>
    public class CouponAppliedDto
    {
        public int CouponId { get; set; }

        public string Code { get; set; } = string.Empty;

        public int PromotionId { get; set; }

        public string PromotionName { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }

        public decimal DiscountAmount { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsExpired { get; set; }
    }

    /// <summary>
    /// DTO kết quả tính toán khuyến mãi cho một sản phẩm
    /// </summary>
    public class PromotionCalculationResult
    {
        public int ProductId { get; set; }

        public decimal OriginalPrice { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Tổng tiền gốc
        /// </summary>
        public decimal OriginalTotal { get; set; }

        /// <summary>
        /// Tiền khuyến mãi
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Giá sau khuyến mãi
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Tổng tiền sau khuyến mãi
        /// </summary>
        public decimal FinalTotal { get; set; }

        /// <summary>
        /// Danh sách khuyến mãi áp dụng
        /// </summary>
        public List<AppliedPromotionDto>? AppliedPromotions { get; set; }
    }

    /// <summary>
    /// DTO kiểm tra tính hợp lệ của khuyến mãi
    /// </summary>
    public class PromotionValidationDto
    {
        public int PromotionId { get; set; }

        public bool IsValid { get; set; }

        public string? Message { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsExpired { get; set; }

        public bool IsNotStarted { get; set; }

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO kiểm tra mã coupon
    /// </summary>
    public class CouponValidationDto
    {
        public int CouponId { get; set; }

        public string Code { get; set; } = string.Empty;

        public bool IsValid { get; set; }

        public string? Message { get; set; }

        public bool IsExpired { get; set; }

        public bool IsUsed { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int PromotionId { get; set; }

        public string PromotionName { get; set; } = string.Empty;

        public string DiscountType { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }
    }
}
