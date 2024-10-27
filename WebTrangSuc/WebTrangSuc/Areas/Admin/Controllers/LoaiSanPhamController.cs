using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class LoaiSanPhamController : Controller
    {
        // GET: Admin/LoaiSanPham
        shoptrangsucEntities1 db = new shoptrangsucEntities1();
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult Create()
        {
            var loaiSP = new LoaiSanPham(); // Khởi tạo một thể hiện mới của Category
            return View(loaiSP);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoaiSanPham loaiSP, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Lấy tên file gốc và tạo đường dẫn lưu vào thư mục "img"
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string path = Server.MapPath("~/img/" + fileName);

                // Lưu file vào thư mục "img"
                file.SaveAs(path);

                // Lưu đường dẫn vào thuộc tính HinhLoaiSanPham
                loaiSP.HinhLoaiSanPham = "/img/" + fileName;
            }

            if (ModelState.IsValid)
            {
                db.LoaiSanPhams.Add(loaiSP);
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
            }

            return View(loaiSP); // Trả về view nếu có lỗi
        }

    }
}