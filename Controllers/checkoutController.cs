using Microsoft.AspNetCore.Mvc;
using YourProjectName.Models;

namespace YourProjectName.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
       
        private static List<CartItem> _currentCart = new List<CartItem>(); 
        private static List<Order> _orders = new List<Order>(); 

        [HttpPost("process")]
        public IActionResult ProcessCheckout([FromBody] CheckoutRequest request)
        {
            
            if (!_currentCart.Any()) 
                return BadRequest("Giỏ hàng của bạn đang trống, không thể thanh toán.");

            
            var newOrder = new Order
            {
                OrderId = _orders.Count + 1,
                OrderDate = DateTime.Now,
                CustomerName = request.CustomerName,
                ShippingAddress = request.ShippingAddress,
                OrderDetails = _currentCart.Select(c => new OrderDetail
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    UnitPrice = c.Price,
                    Quantity = c.Quantity
                }).ToList(),
                TotalAmount = _currentCart.Sum(x => x.Price * x.Quantity)
            };

            _orders.Add(newOrder);

            _currentCart.Clear();

            return Ok(new 
            { 
                Message = "Thanh toán thành công!", 
                OrderId = newOrder.OrderId, 
                Total = newOrder.TotalAmount 
            });
        }

        [HttpGet("history")]
        public IActionResult GetOrderHistory() => Ok(_orders);
    }
}