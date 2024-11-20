using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class ThuongHieuController : Controller
    {
        private readonly shoptrangsucEntities1 db = new shoptrangsucEntities1();

        // GET: ThuongHieu
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var thuongHieus = db.ThuongHieux.ToList().ToPagedList(pageNum, pageSize);
            return View(thuongHieus);
        }

        // GET: ThuongHieu/Create
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThuongHieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create([Bind(Include = "ID,TenThuongHieu")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                db.ThuongHieux.Add(thuongHieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuongHieu);
        }

        // GET: ThuongHieu/Edit/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuongHieu thuongHieu = db.ThuongHieux.Find(id);
            if (thuongHieu == null)
            {
                return HttpNotFound();
            }
            return View(thuongHieu);
        }

        // POST: ThuongHieu/Edit/5
        [HttpPost]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TenThuongHieu")] ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuongHieu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuongHieu);
        }

        // GET: ThuongHieu/Delete/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuongHieu thuongHieu = db.ThuongHieux.Find(id);
            if (thuongHieu == null)
            {
                return HttpNotFound();
            }
            return View(thuongHieu);
        }

        // POST: ThuongHieu/Delete/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThuongHieu thuongHieu = db.ThuongHieux.Find(id);
            db.ThuongHieux.Remove(thuongHieu);
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
