using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using WebTrangSuc.Models;
using BCrypt.Net;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTrangSuc.Controllers
{
    public class DangKyController : ApiController
    {
        private readonly shoptrangsucEntities1 _context;

        public DangKyController()
        {
            _context = new shoptrangsucEntities1();
        }

        public DangKyController(shoptrangsucEntities1 context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("api/taikhoan/dangky")]
        public async Task<IHttpActionResult> DangKy(TaiKhoanModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra email hợp lệ
            if (!IsValidEmail(model.Email))
            {
                return BadRequest("Email không hợp lệ.");
            }

            // Kiểm tra mật khẩu mạnh
            if (!IsStrongPassword(model.Matkhau))
            {
                return BadRequest("Mật khẩu phải có ít nhất 8 ký tự, 1 chữ cái in hoa và 1 ký tự đặc biệt.");
            }

            if (model.XacNhanMatKhau != model.Matkhau)
            {
                return BadRequest("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            // Kiểm tra trùng UserName hoặc Email
            if (_context.TaiKhoans.Any(x => x.UserName == model.UserName || x.Email == model.Email))
            {
                return BadRequest("Tên đăng nhập hoặc Email đã tồn tại.");
            }
            
            var taiKhoan = new TaiKhoan
            {
                HoVaTen = model.HoVaTen,
                GioiTinh = model.GioiTinh,
                NamSinh = model.NamSinh,
                SDT = model.SDT,
                Email = model.Email,
                UserName = model.UserName,
                Matkhau = BCrypt.Net.BCrypt.HashPassword(model.Matkhau),
                Avatar = model.Avatar,
                IDRole = 4 
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }
        // Hàm kiểm tra email hợp lệ
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email,
                      @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        // Hàm kiểm tra mật khẩu mạnh
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[!@#$&*])(?=.{8,})");
        }
    }
}