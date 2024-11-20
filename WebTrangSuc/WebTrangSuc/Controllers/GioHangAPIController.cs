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
        [Route("api/cart/add")]
        public async Task<IHttpActionResult> AddToCart([FromBody] AddToCartDto model)
        {
            if (model == null || model.IDUser <= 0 || model.IDSanPham <= 0 || model.SoLuongSanPham <= 0)
            {
                // Ghi log kiểm tra dữ liệu
                Console.WriteLine($"Dữ liệu không hợp lệ: IDUser={model?.IDUser}, IDSanPham={model?.IDSanPham}, SoLuongSanPham={model?.SoLuongSanPham}");
                return BadRequest("Thông tin không hợp lệ");
            }

            try
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
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi thêm sản phẩm vào giỏ hàng", ex));
            }
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

        [HttpPost]
        [Route("api/cart/applyvoucher")]
        public async Task<IHttpActionResult> ApplyVoucher([FromBody] ApplyVoucherDto model)
        {
            if (model == null || model.UserId <= 0 || string.IsNullOrEmpty(model.VoucherCode))
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            try
            {
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.TenVoucher == model.VoucherCode);
                        //&& v.NgayBatDau <= DateTime.Now
                        //&& v.NgayKetThuc >= DateTime.Now);

                if (voucher == null)
                {
                    return BadRequest("Mã voucher không hợp lệ hoặc đã hết hạn.");
                }

                if (voucher.SoLuongSuDung <= 0)
                {
                    return BadRequest("Voucher đã hết lượt sử dụng.");
                }

                // Giảm giá
                var cartItems = await _context.SanPhamGioHangs
                    .Where(c => c.IDUser == model.UserId)
                    .Include(c => c.SanPham)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest("Giỏ hàng trống.");
                }

                var totalPrice = cartItems.Sum(item => item.SoLuongSanPham * item.SanPham.Gia);
                var discountAmount = (totalPrice * voucher.GiaVoucher) / 100;
                var newTotalPrice = totalPrice - discountAmount;

                // Cập nhật số lượng sử dụng của voucher
                voucher.SoLuongSuDung--;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    OriginalPrice = totalPrice,
                    DiscountAmount = discountAmount,
                    TotalPriceAfterDiscount = newTotalPrice,
                    Message = "Áp dụng voucher thành công."
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi áp dụng voucher.", ex));
            }
        }


       

    }
}
