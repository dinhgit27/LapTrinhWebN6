using Microsoft.AspNetCore.Mvc;
using FashionEcommerce.Models;      
using FashionEcommerce.Models.DTOs; 
using FashionEcommerce.DTOs;        
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        // Sử dụng static để lưu tạm dữ liệu trên RAM (Sẽ mất khi restart server)
        private static List<Order> _orders = new List<Order>(); 

        [HttpPost("process")]
        public IActionResult ProcessCheckout([FromBody] CheckoutRequest request)
        {
            // 1. Kiểm tra danh sách hàng gửi lên từ Client
            if (request.Items == null || !request.Items.Any()) 
            {
                return BadRequest(new { message = "Giỏ hàng của bạn đang trống, không thể thanh toán." });
            }

            try 
            {
                // 2. Tạo đối tượng đơn hàng mới
                var newOrder = new Order
                {
                    OrderId = _orders.Count + 1,
                    OrderDate = DateTime.Now,
                    CustomerName = request.CustomerName,
                    ShippingAddress = request.ShippingAddress,
                    
                    // 3. Chuyển đổi từ danh sách sản phẩm trong giỏ sang OrderDetail (Khớp với Model OrderDetail của bạn)
                    OrderDetails = request.Items.Select(c => new OrderDetail 
                    {
                        ProductVariantId = c.ProductId, 
                        Snapshot_ProductName = c.ProductName, // Khớp với trường Snapshot trong Model của bạn
                        Snapshot_Sku = "N/A",                 // Bạn có thể bổ sung SKU vào CartItem nếu cần
                        Snapshot_Thumbnail = c.Thumbnail,     // Lưu ảnh tại thời điểm đặt
                        UnitPrice = c.Price,                  // Giá tại thời điểm đặt
                        Quantity = c.Quantity                 // Số lượng đặt
                    }).ToList(),

                    // 4. Tính tổng tiền
                    TotalAmount = request.Items.Sum(x => x.Price * x.Quantity)
                };

                // 5. Lưu vào danh sách tạm
                _orders.Add(newOrder);

                return Ok(new { 
                    message = "Thanh toán thành công!", 
                    orderId = newOrder.OrderId, 
                    total = newOrder.TotalAmount 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi xử lý đơn hàng", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử đơn hàng đã đặt (Lưu trên RAM)
        /// </summary>
        [HttpGet("history")]
        public IActionResult GetOrderHistory() 
        {
            return Ok(_orders);
        }
    }
}