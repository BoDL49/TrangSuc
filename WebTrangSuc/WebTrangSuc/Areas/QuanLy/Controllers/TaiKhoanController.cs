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

namespace WebTrangSuc.Areas.QuanLy.Controllers
{
    public class TaiKhoanController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        // GET: Admin/TaiKhoan
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var taiKhoans = db.TaiKhoans.Include(t => t.Role).AsQueryable();    


            // Tìm kiếm chính xác theo tên người dùng hoặc tên role
            if (!string.IsNullOrEmpty(SearchString))
            {
                taiKhoans = taiKhoans.Where(s => s.UserName.Contains(SearchString) ||
                                 s.Role.TenRole.Contains(SearchString)
                                 || s.HoVaTen.Contains(SearchString));
            }

            return View(taiKhoans.OrderBy(s => s.ID).ToPagedList(pageNum, pageSize));
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

        // GET: Admin/TaiKhoan/Create
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create()
        {
            // Lấy danh sách Role từ cơ sở dữ liệu và truyền vào ViewBag
            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole");
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create([Bind(Include = "HoVaTen,GioiTinh,NamSinh,SDT,Email,UserName,Matkhau,Avatar,IDRole")] TaiKhoan taiKhoan, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload avatar (nếu có)
                if (Avatar != null)
                {
                    string fileName = System.IO.Path.GetFileName(Avatar.FileName);
                    string path = Server.MapPath("~/img/" + fileName);
                    Avatar.SaveAs(path);
                    taiKhoan.Avatar = "/img/" + fileName;
                }

                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole", taiKhoan.IDRole);
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Edit/5
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

            // Khởi tạo ViewBag.IDRole với danh sách Roles từ cơ sở dữ liệu
            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole", taiKhoan.IDRole);
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
                    // Lấy entity hiện có từ database
                    var existingTaiKhoan = db.TaiKhoans.Find(taiKhoan.ID);
                    if (existingTaiKhoan == null)
                    {
                        return HttpNotFound();
                    }

                    // Cập nhật các trường thông tin
                    existingTaiKhoan.HoVaTen = taiKhoan.HoVaTen;
                    existingTaiKhoan.GioiTinh = taiKhoan.GioiTinh;
                    existingTaiKhoan.NamSinh = taiKhoan.NamSinh;
                    existingTaiKhoan.SDT = taiKhoan.SDT;
                    existingTaiKhoan.Email = taiKhoan.Email;
                    existingTaiKhoan.UserName = taiKhoan.UserName;
                    existingTaiKhoan.IDRole = taiKhoan.IDRole;

                    // Chỉ cập nhật mật khẩu nếu có nhập
                    if (!string.IsNullOrEmpty(taiKhoan.Matkhau))
                    {
                        existingTaiKhoan.Matkhau = taiKhoan.Matkhau;
                    }

                    // Xử lý avatar
                    if (Avatar != null)
                    {
                        string fileName = Path.GetFileName(Avatar.FileName);
                        string path = Server.MapPath("~/img/" + fileName);
                        Avatar.SaveAs(path);
                        existingTaiKhoan.Avatar = "/img/" + fileName;
                    }

                    db.Entry(existingTaiKhoan).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    // Xử lý hiển thị lỗi validation
                    var errorMessages = ex.EntityValidationErrors
                                        .SelectMany(x => x.ValidationErrors)
                                        .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    ModelState.AddModelError("", "Lỗi validation: " + fullErrorMessage);
                }
            }

            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole", taiKhoan.IDRole);
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