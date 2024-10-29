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
    public class LoaiSanPhamController : Controller
    {
        // GET: Admin/LoaiSanPham
        shoptrangsucEntities1 db = new shoptrangsucEntities1();
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var loaiSP = db.LoaiSanPhams.ToList().ToPagedList(pageNum, pageSize);

            return View(loaiSP);
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

            if (ModelState.IsValid)
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

                db.LoaiSanPhams.Add(loaiSP);
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
            }

            return View(loaiSP); // Trả về view nếu có lỗi
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Lấy loại sản phẩm theo ID để hiển thị trong form Edit
            var loaiSP = db.LoaiSanPhams.Find(id);
            if (loaiSP == null)
            {
                return HttpNotFound();
            }
            return View(loaiSP);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiSanPham loaiSP, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Lấy loại sản phẩm từ DB theo ID để cập nhật
                var existingLoaiSP = db.LoaiSanPhams.Find(loaiSP.ID);
                if (existingLoaiSP == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các thuộc tính của loại sản phẩm
                existingLoaiSP.TenLoaiSanPham = loaiSP.TenLoaiSanPham;

                if (file != null && file.ContentLength > 0)
                {
                    // Lấy tên file gốc và tạo đường dẫn lưu vào thư mục "img"
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string path = Server.MapPath("~/img/" + fileName);

                    // Lưu file vào thư mục "img"
                    file.SaveAs(path);

                    // Lưu đường dẫn vào thuộc tính HinhLoaiSanPham
                    existingLoaiSP.HinhLoaiSanPham = "/img/" + fileName;
                }

                // Đánh dấu đối tượng là đã sửa đổi
                db.Entry(existingLoaiSP).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
            }

            return View(loaiSP); // Trả về view nếu có lỗi
        }

        public ActionResult Delete(int id)
        {
            var loaiSP = db.LoaiSanPhams.Find(id); // Tìm sản phẩm theo ID
            if (loaiSP == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }
            return View(loaiSP); // Trả về view để xác nhận xóa
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var loaiSP = db.LoaiSanPhams.Find(id); // Tìm sản phẩm theo ID
            if (loaiSP != null)
            {
                db.LoaiSanPhams.Remove(loaiSP); // Xóa sản phẩm
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
        }
    }
}