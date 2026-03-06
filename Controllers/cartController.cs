using Microsoft.AspNetCore.Mvc;
using FashionEcommerce.Models;      // Để nhận diện CartItem, Order
using FashionEcommerce.Models.DTOs; // Một số DTO của bạn đang nằm trong namespace này
using FashionEcommerce.DTOs;        // Để nhận diện CartRequest, CheckoutRequest

namespace FashionEcommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
       
        private static List<CartItem> _cart = new List<CartItem>();

        
        [HttpGet]
        public IActionResult GetCart()
        {
            return Ok(new { 
                TotalAmount = _cart.Sum(x => x.Total), 
                Items = _cart 
            });
        }

       
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartRequest request)
        {
            if (request.Quantity <= 0) return BadRequest("Số lượng phải > 0");

            var item = _cart.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (item != null)
            {
                item.Quantity += request.Quantity; // Đã có thì cộng dồn
            }
            else
            {
                // Giả định lấy thông tin từ DB theo ProductId
                _cart.Add(new CartItem 
                { 
                    ProductId = request.ProductId, 
                    ProductName = $"Sản phẩm #{request.ProductId}", 
                    Price = 50000, 
                    Quantity = request.Quantity 
                });
            }
            return Ok(_cart);
        }
        [HttpPut("update")]
        public IActionResult UpdateCart([FromBody] CartRequest request)
        {
            var item = _cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (item == null) return NotFound("Không tìm thấy sản phẩm trong giỏ");

            if (request.Quantity <= 0) 
            {
                _cart.Remove(item);
            }
            else 
            {
                item.Quantity = request.Quantity;
            }
            return Ok(_cart);
        }
        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            var item = _cart.FirstOrDefault(x => x.ProductId == productId);
            if (item == null) return NotFound("Sản phẩm không có trong giỏ");

            _cart.Remove(item);
            return Ok(new { Message = "Đã xóa thành công", Cart = _cart });
        }

        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            _cart.Clear();
            return Ok("Giỏ hàng đã trống");
        }
    }
}