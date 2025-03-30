using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebTrangSuc.Models;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace WebTrangSuc.Controllers.API
{
    [RoutePrefix("api/danhgia")]
    public class DanhGiaAPIController : ApiController
    {
        private readonly shoptrangsucEntities1 _db = new shoptrangsucEntities1();

        [HttpGet]
        [Route("kiemtraquyen/{productId}")]
        public async Task<IHttpActionResult> CheckReviewPermission(int productId)
        {
            try
            {
                // Lấy userId từ header
                var userIdHeader = HttpContext.Current.Request.Headers["userId"];
                if (string.IsNullOrEmpty(userIdHeader) || !int.TryParse(userIdHeader, out int userId))
                {
                    return Unauthorized(); // Không có userId hoặc không hợp lệ
                }

                // Kiểm tra xem người dùng đã mua sản phẩm và đơn hàng đã giao chưa
                var hasPurchased = await _db.DonHangs.AnyAsync(dh =>
                    dh.IDUser == userId &&
                    dh.TrangThaiDonHang == 2 && // Đơn hàng đã giao
                    dh.DonHangChiTiets.Any(ct => ct.IDSanPham == productId));

                return Ok(new { hasPermission = hasPurchased });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("danhgia")]
        public async Task<IHttpActionResult> SubmitReview(DanhGiaRequest request)
        {
            try
            {
                var userIdHeader = HttpContext.Current.Request.Headers["userId"];
                if (string.IsNullOrEmpty(userIdHeader) || !int.TryParse(userIdHeader, out var userId) || userId <= 0)
                {
                    return BadRequest("ID người dùng không hợp lệ.");
                }

                if (request.Rate < 1 || request.Rate > 5)
                    return BadRequest("Đánh giá phải từ 1 đến 5 sao");

                if (string.IsNullOrWhiteSpace(request.NoiDung))
                    return BadRequest("Vui lòng nhập nội dung đánh giá");

                var user = await _db.TaiKhoans.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest("Người dùng không tồn tại.");
                }

                var existingReview = await _db.Database.SqlQuery<int>(
                    "SELECT COUNT(*) FROM DanhGia WHERE IDSanPham = @p0 AND IDUser = @p1",
                    new SqlParameter("@p0", request.IDSanPham),
                    new SqlParameter("@p1", userId)).FirstOrDefaultAsync();

                if (existingReview > 0)
                {
                    return BadRequest("Bạn đã đánh giá sản phẩm này rồi.");
                }

                var tenNguoiDanhGia = user.HoVaTen ?? user.UserName;
                var ngayDanhGia = DateTime.Now;

                var query = "INSERT INTO DanhGia (IDSanPham, IDUser, Rate, NoiDung, TenNguoiDanhGia, NgayDanhGia) " +
                            "VALUES (@IDSanPham, @IDUser, @Rate, @NoiDung, @TenNguoiDanhGia, @NgayDanhGia)";

                var parameters = new[]
                {
                    new SqlParameter("@IDSanPham", request.IDSanPham),
                    new SqlParameter("@IDUser", userId),
                    new SqlParameter("@Rate", request.Rate),
                    new SqlParameter("@NoiDung", request.NoiDung),
                    new SqlParameter("@TenNguoiDanhGia", tenNguoiDanhGia),
                    new SqlParameter("@NgayDanhGia", ngayDanhGia)
                };

                await _db.Database.ExecuteSqlCommandAsync(query, parameters);

                return Ok(new { message = "Đánh giá thành công!" });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 515)
                {
                    return BadRequest("Lỗi khi cập nhật cơ sở dữ liệu. Vui lòng kiểm tra lại dữ liệu đầu vào.");
                }
                return InternalServerError(new Exception("Lỗi khi cập nhật cơ sở dữ liệu. Vui lòng kiểm tra lại dữ liệu đầu vào.", ex));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        [HttpGet]
        [Route("danhsach/{productId}")]
        public async Task<IHttpActionResult> GetReviews(int productId)
        {
            try
            {
                var reviews = await _db.Database.SqlQuery<ReviewDto>(
                    "SELECT TenNguoiDanhGia, Rate, NoiDung, CONVERT(varchar, NgayDanhGia, 103) + ' ' + CONVERT(varchar, NgayDanhGia, 108) AS NgayDanhGia " +
                    "FROM DanhGia WHERE IDSanPham = @p0 " +
                    "ORDER BY NgayDanhGia DESC",
                    productId).ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // Tính điểm đánh giá trung bình
        [HttpGet]
        [Route("diemtrungbinh/{productId}")]
        public async Task<IHttpActionResult> GetAverageRating(int productId)
        {
            try
            {
                var averageRating = await _db.DanhGias
                    .Where(dg => dg.IDSanPham == productId)
                    .AverageAsync(dg => (double?)dg.Rate) ?? 0;

                return Ok(new { averageRating = Math.Round(averageRating, 1) });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class DanhGiaRequest
    {
        public int IDSanPham { get; set; }
        public int Rate { get; set; }
        public string NoiDung { get; set; }
    }

    public class ReviewDto
    {
        public string TenNguoiDanhGia { get; set; }
        public int Rate { get; set; }
        public string NoiDung { get; set; }
        public string NgayDanhGia { get; set; }
        public string Avatar { get; set; }
        public int IDSanPham { get; set; }
    }
}
