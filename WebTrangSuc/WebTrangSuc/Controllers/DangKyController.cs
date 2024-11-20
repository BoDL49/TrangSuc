using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebTrangSuc.Models;
using BCrypt.Net;
using System.Text.RegularExpressions;
using System.Data.Entity;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using System.Web.Helpers;
using WebTrangSuc.Helpers;
using System.Web;
using System.IO;

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

            string hashPassword = Crypto.HashPassword(model.Matkhau);

            var taiKhoan = new TaiKhoan
            {
                HoVaTen = model.HoVaTen,
                GioiTinh = model.GioiTinh,
                NamSinh = model.NamSinh,
                SDT = model.SDT,
                Email = model.Email,
                UserName = model.UserName,
                Matkhau = hashPassword,
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

        [HttpPost]
        [Route("api/taikhoan/dangnhap")]
        public async Task<IHttpActionResult> DangNhap(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.UserName == model.Username || x.Email == model.Username);

            if (taiKhoan == null)
            {
                return BadRequest("Tên đăng nhập không đúng.");
            }

            if (!Crypto.VerifyHashedPassword(taiKhoan.Matkhau, model.Password))
            {
                return BadRequest("Mật khẩu không đúng.");
            }

            var token = JwtHelper.GenerateToken(taiKhoan.ID, Convert.ToInt16(taiKhoan.IDRole));

            return Ok(new
            {
                token,
                userId = taiKhoan.ID,
                role = taiKhoan.IDRole
            });

        }


        [HttpGet]
        [Route("api/taikhoan/{id}")]
        public async Task<IHttpActionResult> GetTaiKhoanById(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.ID == id);

            if (taiKhoan == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = taiKhoan.ID,
                hoVaTen = taiKhoan.HoVaTen,
                gioiTinh = taiKhoan.GioiTinh,
                namSinh = taiKhoan.NamSinh,
                sdt = taiKhoan.SDT,
                email = taiKhoan.Email,
                userName = taiKhoan.UserName,
                avatar = taiKhoan.Avatar,
                idrole = taiKhoan.IDRole
            });
        }

        [HttpGet]
        [Route("api/taikhoan/diachi/{id}")]
        public async Task<IHttpActionResult> GetDiaChiById(int id)
        {
            var diaChi = await _context.DiaChis.FirstOrDefaultAsync(d => d.IDUser == id);

            if (diaChi == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                DiaChi = diaChi.Diachi
            });
        }

        [HttpPut]
        [Route("api/taikhoan/{id}")]
        public async Task<IHttpActionResult> CapNhatThongTin(int id, TaiKhoanModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.ID == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            taiKhoan.HoVaTen = model.HoVaTen;
            taiKhoan.GioiTinh = model.GioiTinh;
            taiKhoan.NamSinh = model.NamSinh;
            taiKhoan.SDT = model.SDT;
            taiKhoan.Email = model.Email;
            taiKhoan.UserName = model.UserName;

            // Kiểm tra nếu mật khẩu mới được gửi
            if (!string.IsNullOrEmpty(model.Matkhau))
            {
                taiKhoan.Matkhau = Crypto.HashPassword(model.Matkhau); // Mã hóa mật khẩu trước khi lưu
            }

            _context.Entry(taiKhoan).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông tin thành công!" });
        }


        private string SaveBase64ToFile(string base64String, string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath($"~/img/UploadedFiles/{fileName}");
            byte[] bytes = Convert.FromBase64String(base64String);
            File.WriteAllBytes(filePath, bytes);
            return $"/UploadedFiles/{fileName}"; // Trả về đường dẫn lưu file
        }


        [HttpPut]
        [Route("api/taikhoan/diachi/{id}")]
        public async Task<IHttpActionResult> CapNhatDiaChi(int id, DiaChi diaChi)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var DiaChi = await _context.DiaChis.FirstOrDefaultAsync(d => d.IDUser == id);
            if (DiaChi == null)
            {
                return NotFound();
            }

            DiaChi.Diachi = diaChi.Diachi;

            return Ok(new { message = "Cập nhật thành công!" });

        }


        [HttpPut]
        [Route("api/taikhoan/doi-mat-khau/{id}")]
        public async Task<IHttpActionResult> DoiMatKhau(int id, DoiMatKhauModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.ID == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            // Kiểm tra mật khẩu cũ
            if (!Crypto.VerifyHashedPassword(taiKhoan.Matkhau, model.MatKhauCu))
            {
                return BadRequest("Mật khẩu cũ không đúng.");
            }

            // Kiểm tra mật khẩu mới
            if (!IsStrongPassword(model.MatKhauMoi))
            {
                return BadRequest("Mật khẩu mới không hợp lệ.");
            }

            // Cập nhật mật khẩu mới
            taiKhoan.Matkhau = Crypto.HashPassword(model.MatKhauMoi);

            _context.Entry(taiKhoan).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công!" });
        }


        [HttpPost]
        [Route("api/taikhoan/upload-avatar/{userId}")]
        public async Task<IHttpActionResult> UploadAvatar(int userId)
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0)
            {
                return BadRequest("Không có tệp nào được tải lên.");
            }

            var file = httpRequest.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                return BadRequest("Tệp tải lên không hợp lệ.");
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Chỉ chấp nhận các định dạng tệp JPG, JPEG, PNG, GIF.");
            }

            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = HttpContext.Current.Server.MapPath($"~/img/UploadedFiles/{fileName}");

            try
            {
                // Lưu file vào server
                file.SaveAs(filePath);

                // Cập nhật đường dẫn avatar vào cơ sở dữ liệu
                var user = await _context.TaiKhoans.FindAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                user.Avatar = $"/img/UploadedFiles/{fileName}";
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { avatarPath = user.Avatar });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Lỗi khi lưu ảnh.", ex));
            }
        }

    }
}
