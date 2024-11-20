using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebTrangSuc.Models;
using System.Data.Entity;

namespace WebTrangSuc.Controllers
{
    public class GioHangAPIController : ApiController
    {
        private readonly shoptrangsucEntities1 _context;

        public GioHangAPIController()
        {
            _context = new shoptrangsucEntities1();
        }

        public GioHangAPIController(shoptrangsucEntities1 context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/cart/{userId}")]
        public async Task<IHttpActionResult> GetCartItems(int userId)
        {
            var cartItems = await _context.SanPhamGioHangs
                .Where(c => c.IDUser == userId)
                .Include(c => c.SanPham)
                .Select(c => new {
                    c.ID,
                    c.IDSanPham,
                    c.SoLuongSanPham,
                    ProductName = c.SanPham.TenSanPham,
                    Price = c.SanPham.Gia,
                    Image = c.SanPham.HinhSanPhams.FirstOrDefault().HinhSP
                }).ToListAsync();

            return Ok(cartItems);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IHttpActionResult> AddToCart([FromBody] AddToCartDto model)
        {
            var existingItem = await _context.SanPhamGioHangs
                .FirstOrDefaultAsync(c => c.IDUser == model.IDUser && c.IDSanPham == model.IDSanPham);

            if (existingItem == null)
            {
                var cartItem = new SanPhamGioHang
                {
                    IDUser = model.IDUser,
                    IDSanPham = model.IDSanPham,
                    SoLuongSanPham = model.SoLuongSanPham
                };
                _context.SanPhamGioHangs.Add(cartItem);
            }
            else
            {
                existingItem.SoLuongSanPham += model.SoLuongSanPham;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Thêm sản phẩm vào giỏ hàng thành công" });
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPut]
        [Route("api/cart/update/{id}")]
        public async Task<IHttpActionResult> UpdateCartItem(int id, [FromBody] UpdateCartDto model)
        {
            var cartItem = await _context.SanPhamGioHangs.FindAsync(id);

            if (cartItem == null)
                return Content(HttpStatusCode.NotFound, new { Message = "Không tìm thấy sản phẩm trong giỏ hàng" });

            cartItem.SoLuongSanPham = model.SoLuongSanPham;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật giỏ hàng thành công" });
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete]
        [Route("api/cart/remove/{id}")]
        public async Task<IHttpActionResult> RemoveCartItem(int id)
        {
            var cartItem = await _context.SanPhamGioHangs.FindAsync(id);

            if (cartItem == null)
                return Content(HttpStatusCode.NotFound, new { Message = "Không tìm thấy sản phẩm trong giỏ hàng" });

            _context.SanPhamGioHangs.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Xóa sản phẩm khỏi giỏ hàng thành công" });
        }
    }
}
