using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using PagedList;
using PagedList.Mvc;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class VoucherController : Controller
    {
        // GET: Admin/Voucher
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var vouCher = db.Vouchers.ToList().ToPagedList(pageNum, pageSize);

            return View(vouCher);
        }
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create()
        {
            var vouCher = new Voucher(); // Khởi tạo một thể hiện mới của Category
            return View(vouCher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
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
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
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
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voucher vouCher)
        {
            if (ModelState.IsValid)
            {
                var existingvouCher = db.Vouchers.Find(vouCher.ID);
                if (existingvouCher == null)
                {
                    return HttpNotFound();
                }

                // Log giá trị trước khi cập nhật
                System.Diagnostics.Debug.WriteLine($"Before Update - TenVoucher: {existingvouCher.TenVoucher}, MoTa: {existingvouCher.Mota}, SoLuong: {existingvouCher.SoLuongSuDung}");

                existingvouCher.TenVoucher = vouCher.TenVoucher;
                existingvouCher.Mota = vouCher.Mota;
                existingvouCher.NgayBatDau = vouCher.NgayBatDau;
                existingvouCher.NgayKetThuc = vouCher.NgayKetThuc;
                existingvouCher.GiaTri = vouCher.GiaTri;
                existingvouCher.SoLuongSuDung = vouCher.SoLuongSuDung;

                db.Entry(existingvouCher).State = EntityState.Modified;
                db.SaveChanges();

                // Log giá trị sau khi cập nhật
                System.Diagnostics.Debug.WriteLine($"After Update - TenVoucher: {existingvouCher.TenVoucher}, MoTa: {existingvouCher.Mota}, SoLuong: {existingvouCher.SoLuongSuDung}");

                return RedirectToAction("Index");
            }

            return View(vouCher);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
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