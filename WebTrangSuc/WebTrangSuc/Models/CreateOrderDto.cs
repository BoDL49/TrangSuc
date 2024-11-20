using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTrangSuc.Models
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int? VoucherId { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public string TenKhachHang { get; set; }
        public string diaChi { get; set; }
        public string email { get; set; }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}