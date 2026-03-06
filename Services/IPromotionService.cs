using FashionEcommerce.Models;
using FashionEcommerce.Models.DTOs;

namespace FashionEcommerce.Services
{
    /// <summary>
    /// Interface cho Promotion Engine - tính toán và áp dụng khuyến mãi
    /// </summary>
    public interface IPromotionService
    {
        /// <summary>
        /// Lấy danh sách khuyến mãi áp dụng được cho sản phẩm
        /// </summary>
        Task<List<Promotion>> GetApplicablePromotionsForProduct(int productId);

        /// <summary>
        /// Tính toán giảm giá cho một sản phẩm
        /// </summary>
        Task<decimal> CalculateDiscountForProduct(int productId, decimal basePrice, int quantity = 1);

        /// <summary>
        /// Lấy khuyến mãi có mức giảm cao nhất cho sản phẩm
        /// </summary>
        Task<Promotion?> GetBestPromotionForProduct(int productId, decimal basePrice);

        /// <summary>
        /// Xác thực coupon và lấy thông tin khuyến mãi
        /// </summary>
        Task<CouponValidationDto> ValidateCoupon(string couponCode, int userId);

        /// <summary>
        /// Kiểm tra xem khuyến mãi có hợp lệ không (ngày + điều kiện)
        /// </summary>
        Task<bool> IsPromotionValid(int promotionId, decimal? totalAmount = null);

        /// <summary>
        /// Tính toán tổng giảm giá cho đơn hàng
        /// </summary>
        Task<OrderPromotionDto> CalculateOrderDiscount(
            List<(int ProductId, int Quantity, decimal UnitPrice)> orderItems,
            int? userId = null,
            string? couponCode = null);

        /// <summary>
        /// Áp dụng khuyến mãi cho giá cơ bản
        /// </summary>
        decimal ApplyPromotionToPrice(decimal basePrice, Promotion promotion);

        /// <summary>
        /// Lấy tất cả khuyến mãi đang hoạt động
        /// </summary>
        Task<List<Promotion>> GetActivePromotions();

        /// <summary>
        /// Lấy khuyến mãi theo ID
        /// </summary>
        Task<Promotion?> GetPromotionById(int promotionId);

        /// <summary>
        /// Đánh dấu coupon đã sử dụng
        /// </summary>
        Task<bool> MarkCouponAsUsed(string couponCode, int userId);
    }
}
