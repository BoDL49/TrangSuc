using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models; // Nhớ thêm namespace cho model của bạn
using System.Security.Cryptography;
using System.Text;

namespace WebTrangSuc.Controllers
{
    public class AuthController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1(); // Giả sử bạn đang sử dụng Entity Framework

        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            // 1. Tìm tài khoản dựa trên email hoặc username và mật khẩu đã mã hóa
            string hashedPassword = HashPassword(password); // Hàm mã hóa mật khẩu
            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(t => (t.Email == email || t.UserName == email) && t.Matkhau == hashedPassword);

            // 2. Xử lý kết quả
            if (taiKhoan != null)
            {
                // Đăng nhập thành công
                Session["UserID"] = taiKhoan.ID; // Lưu ID người dùng vào session
                return RedirectToAction("Index", "Home"); // Redirect về trang chủ hoặc trang mong muốn
            }
            else
            {
                // Đăng nhập thất bại
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaiKhoan taiKhoan)
        {
            // 1. Kiểm tra xem email hoặc username đã tồn tại chưa
            if (db.TaiKhoans.Any(t => t.Email == taiKhoan.Email || t.UserName == taiKhoan.UserName))
            {
                ViewBag.ErrorMessage = "Email hoặc Username đã tồn tại.";
                return View("Index");
            }

            // 2. Mã hóa mật khẩu
            taiKhoan.Matkhau = HashPassword(taiKhoan.Matkhau);

            // 3. Lưu tài khoản mới vào database
            db.TaiKhoans.Add(taiKhoan);
            db.SaveChanges();

            // 4. Chuyển hướng hoặc trả về kết quả
            return RedirectToAction("Index", "TrangChu"); // Chuyển hướng về trang đăng nhập
        }

        // 3. Hàm mã hóa mật khẩu (SHA256)
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}