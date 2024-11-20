using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class TaiKhoanController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        // GET: Admin/TaiKhoan
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var taiKhoans = db.TaiKhoans.Include("Role").ToList().ToPagedList(pageNum, pageSize);
            return View(taiKhoans);
        }

        // GET: Admin/TaiKhoan/Details/5
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
        public ActionResult Create()
        {
            // Lấy danh sách Role từ cơ sở dữ liệu và truyền vào ViewBag
            ViewBag.IDRole = new SelectList(db.Roles, "ID", "TenRole");
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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
