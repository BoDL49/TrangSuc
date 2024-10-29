using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using PagedList;
using PagedList.Mvc;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var sanPham = db.SanPhams.ToList().ToPagedList(pageNum, pageSize);

            return View(sanPham);
        }
        public ActionResult Create()
        {
            var loaiSanPhams = db.LoaiSanPhams.ToList();

            // Tạo SelectList cho dropdown
            ViewBag.LoaiSanPhamList = new SelectList(loaiSanPhams, "ID", "TenLoaiSanPham");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu người dùng đã upload file
                if (file != null && file.ContentLength > 0)
                {
                    // Kiểm tra loại file (để chấp nhận chỉ hình ảnh)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string extension = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file hình ảnh (jpg, jpeg, png).");
                        ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams.ToList(), "ID", "TenLoaiSanPham");
                        return View(sanPham);
                    }

                    // Tạo đường dẫn lưu file trong thư mục "img"
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/img/" + fileName);

                    // Kiểm tra nếu thư mục chưa tồn tại, thì tạo thư mục
                    if (!System.IO.Directory.Exists(Server.MapPath("~/img")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/img"));
                    }

                    // Lưu file vào thư mục "img"
                    file.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính HinhSanPham
                    sanPham.HinhSanPham = "/img/" + fileName;
                }

                // Kiểm tra và gán giá trị mặc định nếu cần
                sanPham.NgayTaoSanPham = sanPham.NgayTaoSanPham ?? DateTime.Now;
                sanPham.TrangThaiSanPham = sanPham.TrangThaiSanPham; // Checkbox sẽ luôn gửi true/false

                // Thêm sản phẩm vào database và lưu
                db.SanPhams.Add(sanPham);
                db.SaveChanges();

                // Chuyển hướng về trang danh sách sản phẩm
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, trả về view với danh sách loại sản phẩm
            ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams.ToList(), "ID", "TenLoaiSanPham");
            return View(sanPham);
        }

        public ActionResult Edit(int id)
        {
            // Lấy sản phẩm từ database
            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            // Lấy danh sách loại sản phẩm để hiển thị trong dropdown
            var loaiSanPhams = db.LoaiSanPhams.ToList();
            ViewBag.LoaiSanPhamList = new SelectList(loaiSanPhams, "ID", "TenLoaiSanPham", sanPham.IDLoaiSanPham);

            return View(sanPham); // Trả về view cùng với sản phẩm
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sanPham, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu người dùng đã upload file
                if (file != null && file.ContentLength > 0)
                {
                    // Kiểm tra loại file (để chấp nhận chỉ hình ảnh)
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    string extension = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file hình ảnh (jpg, jpeg, png).");
                        ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams.ToList(), "ID", "TenLoaiSanPham", sanPham.IDLoaiSanPham);
                        return View(sanPham);
                    }

                    // Tạo đường dẫn lưu file trong thư mục "img"
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/img/" + fileName);

                    // Kiểm tra nếu thư mục chưa tồn tại, thì tạo thư mục
                    if (!System.IO.Directory.Exists(Server.MapPath("~/img")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/img"));
                    }

                    // Lưu file vào thư mục "img"
                    file.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính HinhSanPham
                    sanPham.HinhSanPham = "/img/" + fileName;
                }

                // Cập nhật thông tin sản phẩm trong database
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();

                // Chuyển hướng về trang danh sách sản phẩm
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, trả về view với danh sách loại sản phẩm
            ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams.ToList(), "ID", "TenLoaiSanPham", sanPham.IDLoaiSanPham);
            return View(sanPham);
        }
        public ActionResult Delete(int id)
        {
            // Lấy sản phẩm từ database
            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            return View(sanPham); // Trả về view xác nhận xóa
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm sản phẩm cần xóa
            var sanPham = db.SanPhams.Find(id);
            if (sanPham != null)
            {
                db.SanPhams.Remove(sanPham); // Xóa sản phẩm
                db.SaveChanges(); // Lưu thay đổi vào database
            }

            // Chuyển hướng về trang danh sách sản phẩm
            return RedirectToAction("Index");
        }
    }
}