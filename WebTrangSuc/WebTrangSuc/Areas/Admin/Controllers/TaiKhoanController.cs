using PagedList;
using System;
using System.Collections.Generic;
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

        // GET: Admin/TaiKhoan
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            // Lấy danh sách tài khoản
            var taiKhoans = db.TaiKhoans.AsQueryable();

            // Tìm kiếm chính xác theo tên người dùng hoặc tên role
            if (!string.IsNullOrEmpty(SearchString))
            {
                taiKhoans = taiKhoans.Where(s => s.UserName.Equals(SearchString, StringComparison.OrdinalIgnoreCase) ||
                                 s.Role.TenRole.Equals(SearchString, StringComparison.OrdinalIgnoreCase));

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
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
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
            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole", taiKhoan.IDRole);
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,HoVaTen,GioiTinh,NamSinh,SDT,Email,UserName,Matkhau,Avatar,IDRole")] TaiKhoan taiKhoan, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {
                if (Avatar != null)
                {
                    string fileName = System.IO.Path.GetFileName(Avatar.FileName);
                    string path = Server.MapPath("~/img/" + fileName);
                    Avatar.SaveAs(path);
                    taiKhoan.Avatar = "/img/" + fileName;
                }

                db.Entry(taiKhoan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole", taiKhoan.IDRole);
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Delete/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Delete(int? id)
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
