using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class DonHangAPIController : ApiController
    {
        private readonly shoptrangsucEntities1 _context;

        public DonHangAPIController()
        {
            _context = new shoptrangsucEntities1();
        }

        public DonHangAPIController(shoptrangsucEntities1 context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("api/donhang/create")]
        public async Task<IHttpActionResult> CreateOrder([FromBody] CreateOrderDto model)
        {
            if (model == null || model.UserId <= 0 || model.CartItems == null || !model.CartItems.Any())
            {
                return BadRequest("Thông tin đơn hàng không hợp lệ.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Tạo đơn hàng
                    var newOrder = new DonHang
                    {
                        IDUser = model.UserId,
                        TongTien = model.TotalAmount,
                        NgayTaoHoaDon = DateTime.Now,
                        TrangThaiGiaoHang = 0, // Mặc định trạng thái giao hàng là chưa giao
                        TrangThaiDonHang = 0, // Mặc định trạng thái đơn hàng là mới
                        IDVoucher = model.VoucherId,
                        PhuongThucThanhToan = model.PaymentMethod
                    };
                    _context.DonHangs.Add(newOrder);
                    await _context.SaveChangesAsync();

                    // Lấy ID đơn hàng vừa tạo
                    var orderId = newOrder.ID;

                    // Tạo chi tiết đơn hàng
                    foreach (var item in model.CartItems)
                    {
                        var orderDetail = new DonHangChiTiet
                        {
                            IDDonHang = orderId,
                            IDSanPham = item.ProductId,
                            SoLuongSanPham = item.Quantity,
                            GiaSanPham = item.Price
                        };
                        _context.DonHangChiTiets.Add(orderDetail);

                        // Cập nhật số lượng tồn kho
                        var product = await _context.SanPhams.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            product.SoLuongTonKho -= item.Quantity;
                        }
                    }

                    // Xóa giỏ hàng sau khi đặt hàng
                    var cartItems = _context.SanPhamGioHangs.Where(c => c.IDUser == model.UserId);
                    _context.SanPhamGioHangs.RemoveRange(cartItems);

                    await _context.SaveChangesAsync();

                    // Commit transaction
                    transaction.Commit();

                    return Ok(new { Message = "Đặt hàng thành công.", OrderId = orderId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return InternalServerError(new Exception("Lỗi khi tạo đơn hàng.", ex));
                }
            }
        }


        [HttpGet]
        [Route("api/donhang/hoadon/{orderId}")]
        public async Task<IHttpActionResult> GetOrderInvoice(int orderId)
        {
            var order = await _context.DonHangs
    .Include("DonHangChiTiets.SanPham")
    .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null)
                return NotFound();

            return Ok(new
            {
                OrderId = order.ID,
                UserId = order.IDUser,
                TotalAmount = order.TongTien,
                OrderDate = order.NgayTaoHoaDon,
                TenKhachHang = order.TaiKhoan.HoVaTen,
                diaChi = order.TaiKhoan.DiaChis.FirstOrDefault(p => p.IDUser == order.TaiKhoan.ID),
                email = order.TaiKhoan.Email,
                Items = order.DonHangChiTiets.Select(d => new
                {
                    ProductName = d.SanPham.TenSanPham,
                    Quantity = d.SoLuongSanPham,
                    Price = d.GiaSanPham
                })
            });
        }

        [HttpGet]
        [Route("api/donhang/get")]
        public async Task<IHttpActionResult> GetOrders(int userId)
        {
            try
            {
                var orders = await _context.DonHangs
                    .Where(o => o.IDUser == userId)
                    .OrderByDescending(o => o.NgayTaoHoaDon)
                    .Select(o => new
                    {
                        OrderId = o.ID,
                        TotalAmount = o.TongTien,
                        OrderDate = o.NgayTaoHoaDon,
                        Status = o.TrangThaiDonHang,
                        DeliveryStatus = o.TrangThaiGiaoHang,
                        TenKhachHang = o.TaiKhoan.HoVaTen
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi lấy danh sách đơn hàng.", ex));
            }
        }


    }
}
