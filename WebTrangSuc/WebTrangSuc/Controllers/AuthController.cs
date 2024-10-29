using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models; 
using System.Security.Cryptography;
using System.Text;

namespace WebTrangSuc.Controllers
{
    public class AuthController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            TaiKhoan taiKhoan = db.TaiKhoans.FirstOrDefault(t => (t.Email == email || t.UserName == email) && t.Matkhau == hashedPassword);

            if (taiKhoan != null)
            {
                Session["UserID"] = taiKhoan.ID;
                Session["RoleName"] = taiKhoan.Role.TenRole;

                switch (taiKhoan.IDRole)
                {
                    case 1:
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    case 2:
                        return RedirectToAction("Index", "Home", new { area = "Xulydonhang" });
                    case 3:
                        return RedirectToAction("Index", "Home", new { area = "Quanly" });
                    case 4:
                        return RedirectToAction("Index", "TrangChu");
                    default:
                        return RedirectToAction("Index", "TrangChu");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return View("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaiKhoan taiKhoan)
        {
            if (db.TaiKhoans.Any(t => t.Email == taiKhoan.Email || t.UserName == taiKhoan.UserName))
            {
                ViewBag.ErrorMessage = "Email hoặc Username đã tồn tại.";
                return View("Index");
            }
            //Console.WriteLine("HoVaTen" + taiKhoan.HoVaTen);
            //Console.WriteLine("Email" + taiKhoan.Email);
            //Console.WriteLine("UserName"

            //taiKhoan.Matkhau = HashPassword(taiKhoan.Matkhau);

            //db.TaiKhoans.Add(taiKhoan);
            //db.SaveChanges();

            return RedirectToAction("Index", "Auth"); 
        }


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