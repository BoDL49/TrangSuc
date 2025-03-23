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

        //[HttpPost]
        //[Route("api/donhang/create")]
        //public async Task<IHttpActionResult> CreateOrder([FromBody] CreateOrderDto model)
        //{
        //    if (model == null || model.UserId <= 0 || model.CartItems == null || !model.CartItems.Any())
        //    {
        //        return BadRequest("Thông tin đơn hàng không hợp lệ.");
        //    }

        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Tạo đơn hàng
        //            var newOrder = new DonHang
        //            {
        //                IDUser = model.UserId,
        //                TongTien = model.TotalAmount,
        //                NgayTaoHoaDon = DateTime.Now,
        //                TrangThaiGiaoHang = 0, // Mặc định chưa giao hàng
        //                TrangThaiDonHang = 0,  // Mặc định trạng thái là mới
        //                IDVoucher = model.VoucherId,
        //                PhuongThucThanhToan = model.PaymentMethod
        //            };
        //            _context.DonHangs.Add(newOrder);
        //            await _context.SaveChangesAsync();

        //            var orderId = newOrder.ID;

        //            // Tạo chi tiết đơn hàng
        //            foreach (var item in model.CartItems)
        //            {
        //                var product = await _context.SanPhams.FindAsync(item.ProductId);
        //                if (product == null)
        //                {
        //                    transaction.Rollback();
        //                    return BadRequest($"Sản phẩm với ID {item.ProductId} không tồn tại.");
        //                }

        //                if (product.SoLuongTonKho < item.Quantity)
        //                {
        //                    transaction.Rollback();
        //                    return BadRequest($"Sản phẩm {product.TenSanPham} không đủ số lượng tồn kho.");
        //                }

        //                var orderDetail = new DonHangChiTiet
        //                {
        //                    IDDonHang = orderId,
        //                    IDSanPham = item.ProductId,
        //                    SoLuongSanPham = item.Quantity,
        //                    GiaSanPham = item.Price
        //                };
        //                _context.DonHangChiTiets.Add(orderDetail);

        //                // Cập nhật tồn kho sản phẩm
        //                product.SoLuongTonKho -= item.Quantity;
        //            }

        //            // Xóa giỏ hàng sau khi tạo đơn hàng
        //            var cartItems = _context.SanPhamGioHangs.Where(c => c.IDUser == model.UserId);
        //            _context.SanPhamGioHangs.RemoveRange(cartItems);

        //            await _context.SaveChangesAsync();
        //            transaction.Commit();

        //            return Ok(new { Message = "Đặt hàng thành công.", OrderId = orderId });
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            Console.WriteLine("Lỗi tạo đơn hàng: " + ex.Message);
        //            if (ex.InnerException != null)
        //            {
        //                Console.WriteLine("Chi tiết lỗi bên trong: " + ex.InnerException.Message);
        //            }
        //            return InternalServerError(new Exception("Lỗi khi tạo đơn hàng.", ex));
        //        }
        //    }
        //}

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
                    // Tạo đơn hàng với trạng thái mặc định là "Mới"
                    var newOrder = new DonHang
                    {
                        IDUser = model.UserId,
                        TongTien = model.TotalAmount,
                        NgayTaoHoaDon = DateTime.Now,
                        TrangThaiGiaoHang = 0,
                        TrangThaiDonHang = 0,
                        IDVoucher = model.VoucherId,
                        PhuongThucThanhToan = model.PaymentMethod
                    };
                    _context.DonHangs.Add(newOrder);
                    await _context.SaveChangesAsync();

                    var orderId = newOrder.ID;

                    // Tạo chi tiết đơn hàng
                    foreach (var item in model.CartItems)
                    {
                        var product = await _context.SanPhams.FindAsync(item.ProductId);
                        if (product == null)
                        {
                            transaction.Rollback();
                            return BadRequest($"Sản phẩm với ID {item.ProductId} không tồn tại.");
                        }

                        if (product.SoLuongTonKho < item.Quantity)
                        {
                            transaction.Rollback();
                            return BadRequest($"Sản phẩm {product.TenSanPham} không đủ số lượng tồn kho.");
                        }

                        var orderDetail = new DonHangChiTiet
                        {
                            IDDonHang = orderId,
                            IDSanPham = item.ProductId,
                            SoLuongSanPham = item.Quantity,
                            GiaSanPham = item.Price
                        };
                        _context.DonHangChiTiets.Add(orderDetail);

                        product.SoLuongTonKho -= item.Quantity;
                    }

                    // Xóa giỏ hàng
                    var cartItems = _context.SanPhamGioHangs.Where(c => c.IDUser == model.UserId);
                    _context.SanPhamGioHangs.RemoveRange(cartItems);

                    await _context.SaveChangesAsync();
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

        [HttpPost]
        [Route("api/donhang/process/{orderId}")]
        public async Task<IHttpActionResult> ProcessOrder(int orderId)
        {
            var order = await _context.DonHangs.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                order.Process(); // Chuyển trạng thái sang "Đang xử lý"
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Đơn hàng đang được xử lý.", Status = order.GetCurrentStatus() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/donhang/deliver/{orderId}")]
        public async Task<IHttpActionResult> DeliverOrder(int orderId)
        {
            var order = await _context.DonHangs.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                order.Deliver(); // Chuyển trạng thái sang "Đã giao"
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Đơn hàng đã được giao.", Status = order.GetCurrentStatus() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("api/donhang/cancel/{orderId}")]
        public async Task<IHttpActionResult> CancelOrder(int orderId)
        {
            var order = await _context.DonHangs.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                order.Cancel(); // Chuyển trạng thái sang "Hủy" thông qua State Pattern
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Đơn hàng đã bị hủy.", Status = order.GetCurrentStatus() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                diaChi = order.TaiKhoan?.DiaChis.FirstOrDefault()?.Diachi,
                email = order.TaiKhoan.Email,
                PaymentMethod = order.PhuongThucThanhToan,
                DeliveryStatus = order.TrangThaiGiaoHang,
                Items = order.DonHangChiTiets.Select(d => new
                {
                    ProductName = d.SanPham?.TenSanPham ?? "N/A",
                    Quantity = d.SoLuongSanPham,
                    Price = d.GiaSanPham
                }).ToList()
            });
        }

        [HttpGet]
        [Route("api/donhang/get")]
        public async Task<IHttpActionResult> GetOrders(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var totalOrders = await _context.DonHangs
                    .Where(o => o.IDUser == userId)
                    .CountAsync();

                var orders = await _context.DonHangs
                    .Where(o => o.IDUser == userId)
                    .OrderByDescending(o => o.NgayTaoHoaDon)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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

                return Ok(new
                {
                    TotalOrders = totalOrders,
                    Page = page,
                    PageSize = pageSize,
                    Orders = orders
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi lấy danh sách đơn hàng.", ex));
            }
        }


        //[HttpGet]
        //[Route("api/donhang/{orderId}")]
        //public async Task<IHttpActionResult> GetOrderDetails(int orderId)
        //{
        //    var order = await _context.DonHangs
        //        .Include(o => o.DonHangChiTiets.Select(c => c.SanPham))
        //        .FirstOrDefaultAsync(o => o.ID == orderId);

        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    var response = new
        //    {
        //        OrderId = order.ID,
        //        UserId = order.IDUser,
        //        TotalPrice = order.TongTien,
        //        PaymentMethod = order.PhuongThucThanhToan,
        //        OrderDate = order.NgayTaoHoaDon,
        //        OrderStatus = order.TrangThaiDonHang,
        //        Items = order.DonHangChiTiets.Select(c => new
        //        {
        //            ProductName = c.SanPham.TenSanPham,
        //            Quantity = c.SoLuongSanPham,
        //            Price = c.GiaSanPham,
        //            Total = c.SoLuongSanPham * c.GiaSanPham
        //        }).ToList()
        //    };

        //    return Ok(response);
        //}

        [HttpGet]
        [Route("api/donhang/{orderId}")]
        public async Task<IHttpActionResult> GetOrderDetails(int orderId)
        {
            var order = await _context.DonHangs
                .Include(o => o.DonHangChiTiets.Select(c => c.SanPham))
                .Include(o => o.TaiKhoan) // Include thông tin tài khoản
                .Include(o => o.TaiKhoan.DiaChis) // Include địa chỉ
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var response = new
            {
                OrderId = order.ID,
                UserId = order.IDUser,
                TotalPrice = order.TongTien,
                PaymentMethod = order.PhuongThucThanhToan,
                OrderDate = order.NgayTaoHoaDon,
                OrderStatus = order.TrangThaiDonHang,
                TenKhachHang = order.TaiKhoan?.HoVaTen ?? "N/A", // Tên khách hàng
                DiaChi = order.TaiKhoan?.DiaChis?.FirstOrDefault()?.Diachi ?? "N/A", 
                Email = order.TaiKhoan?.Email ?? "N/A", // Email
                Items = order.DonHangChiTiets.Select(c => new
                {
                    ProductName = c.SanPham.TenSanPham,
                    Quantity = c.SoLuongSanPham,
                    Price = c.GiaSanPham,
                    Total = c.SoLuongSanPham * c.GiaSanPham
                }).ToList()
            };

            return Ok(response);
        }



    }
}
