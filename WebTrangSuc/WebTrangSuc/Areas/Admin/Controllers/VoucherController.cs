using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Admin/Voucher
        shoptrangsucEntities1 db = new shoptrangsucEntities1();
        public ActionResult Index()
        {
            var vouCher = db.Vouchers.ToList();
            return View(vouCher);
        }
        public ActionResult Create()
        {
            var vouCher = new Voucher(); // Khởi tạo một thể hiện mới của Category
            return View(vouCher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voucher vouCher)
        {

            if (ModelState.IsValid)
            {
                db.Vouchers.Add(vouCher);
                db.SaveChanges();
                return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
            }

            return View(vouCher); // Trả về view nếu có lỗi
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Lấy loại sản phẩm theo ID để hiển thị trong form Edit
            var vouCher = db.Vouchers.Find(id);
            if (vouCher == null)
            {
                return HttpNotFound();
            }
            return View(vouCher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voucher vouCher)
        {
            if (ModelState.IsValid)
            {
                // Lấy loại sản phẩm từ DB theo ID để cập nhật
                var existingvouCher = db.Vouchers.Find(vouCher.ID);
                if (existingvouCher == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật các thuộc tính của loại sản phẩm
                existingvouCher.TenVoucher = vouCher.TenVoucher;

                // Đánh dấu đối tượng là đã sửa đổi
                db.Entry(existingvouCher).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
            }

            return View(vouCher); // Trả về view nếu có lỗi
        }
        public ActionResult Delete(int id)
        {
            // Lấy loại sản phẩm theo ID để hiển thị trong form Edit
            var vouCher = db.Vouchers.Find(id);
            if (vouCher == null)
            {
                return HttpNotFound();
            }
            return View(vouCher);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var vouCher = db.Vouchers.Find(id); // Tìm sản phẩm theo ID
            if (vouCher != null)
            {
                db.Vouchers.Remove(vouCher); // Xóa sản phẩm
                db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
            return RedirectToAction("Index"); // Chuyển hướng về danh sách sản phẩm
        }
    }
}