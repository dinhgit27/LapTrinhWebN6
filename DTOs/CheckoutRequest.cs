using FashionEcommerce.Models;
using System.Collections.Generic;

namespace FashionEcommerce.DTOs
{
    public class CheckoutRequest
    {
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public List<CartItem> Items { get; set; } 
    }
}