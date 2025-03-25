using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class TaiKhoanController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        public TaiKhoanController()
        {
            db = new shoptrangsucEntities1();
            db.Configuration.LazyLoadingEnabled = false; 
        }


        // GET: Admin/TaiKhoan
        // GET: Admin/TaiKhoan
        [RoleAuthorization(1, 2, 3)]
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            // Khởi tạo query base
            var query = db.TaiKhoans
                .Include(t => t.Role)
                .Include(t => t.DiaChis)
                .Include(t => t.DonHangs)
                .AsNoTracking()
                .AsQueryable(); // Chuyển sang IQueryable

            // Áp dụng điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(SearchString))
            {
                var searchLower = SearchString.ToLower();
                query = query.Where(s =>
                    s.UserName.ToLower().Contains(searchLower) ||
                    s.Role.TenRole.ToLower().Contains(searchLower)
                );
            }

            // Sắp xếp sau cùng
            var orderedQuery = query.OrderBy(s => s.ID);

            return View(orderedQuery.ToPagedList(pageNum, pageSize));
        }

        // GET: Admin/TaiKhoan/Create
        [RoleAuthorization(1, 2, 3)]
        public ActionResult Create()
        {
            // Sửa: Thêm ToList() để load dữ liệu ngay lập tức
            ViewBag.IDRole = new SelectList(db.Roles.ToList(), "ID", "TenRole");
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)]
        public ActionResult Create([Bind(Include = "HoVaTen,GioiTinh,NamSinh,SDT,Email,UserName,Matkhau,Avatar,IDRole")] TaiKhoan taiKhoan, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                if (Avatar != null && Avatar.ContentLength > 0)
                {
                    // Sửa: Thêm kiểm tra file hợp lệ
                    string fileName = Path.GetFileName(Avatar.FileName);
                    string path = Path.Combine(Server.MapPath("~/img"), fileName);
                    Avatar.SaveAs(path);
                    taiKhoan.Avatar = "/img/" + fileName;
                }

                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Sửa: Load lại dữ liệu Roles
            ViewBag.IDRole = new SelectList(db.Roles.ToList(), "ID", "TenRole", taiKhoan.IDRole);
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)]
        public ActionResult Edit([Bind(Include = "ID,HoVaTen,GioiTinh,NamSinh,SDT,Email,UserName,Matkhau,Avatar,IDRole")] TaiKhoan taiKhoan, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaiKhoan = db.TaiKhoans.Find(taiKhoan.ID);
                    if (existingTaiKhoan == null)
                    {
                        return HttpNotFound();
                    }

                    // Sửa: Tách logic cập nhật thành phương thức riêng
                    UpdateTaiKhoan(existingTaiKhoan, taiKhoan, Avatar);

                    db.Entry(existingTaiKhoan).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    // Sửa: Ghi log lỗi thay vì xử lý trong debug
                    LogValidationErrors(ex);
                    ModelState.AddModelError("", "Lỗi validation. Vui lòng kiểm tra lại dữ liệu.");
                }
            }

            ViewBag.IDRole = new SelectList(db.Roles.ToList(), "ID", "TenRole", taiKhoan.IDRole);
            return View(taiKhoan);
        }

        private void UpdateTaiKhoan(TaiKhoan existing, TaiKhoan source, HttpPostedFileBase Avatar)
        {
            existing.HoVaTen = source.HoVaTen;
            existing.GioiTinh = source.GioiTinh;
            existing.NamSinh = source.NamSinh;
            existing.SDT = source.SDT;
            existing.Email = source.Email;
            existing.UserName = source.UserName;
            existing.IDRole = source.IDRole;

            if (!string.IsNullOrEmpty(source.Matkhau))
            {
                existing.Matkhau = source.Matkhau;
            }

            if (Avatar != null && Avatar.ContentLength > 0)
            {
                string fileName = Path.GetFileName(Avatar.FileName);
                string path = Path.Combine(Server.MapPath("~/img"), fileName);
                Avatar.SaveAs(path);
                existing.Avatar = "/img/" + fileName;
            }
        }

        private void LogValidationErrors(DbEntityValidationException ex)
        {
            var errorMessages = ex.EntityValidationErrors
                                .SelectMany(x => x.ValidationErrors)
                                .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");

            System.Diagnostics.Debug.WriteLine("Validation errors:");
            foreach (var error in errorMessages)
            {
                System.Diagnostics.Debug.WriteLine(error);
            }
        }
        // GET: Admin/TaiKhoan/Details/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            return View(taiKhoan);
        }

        [RoleAuthorization(1, 2, 3)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            if (taiKhoan == null)
            {
                return HttpNotFound();
            }
            // Sửa: Load roles và tạo SelectList với giá trị selected
            ViewBag.IDRole = new SelectList(db.Roles.ToList(), "ID", "TenRole", taiKhoan.IDRole);
                return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaiKhoan taiKhoan = db.TaiKhoans.Find(id);
            db.TaiKhoans.Remove(taiKhoan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
