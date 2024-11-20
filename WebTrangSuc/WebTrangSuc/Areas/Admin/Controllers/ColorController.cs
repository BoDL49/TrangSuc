using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class ColorController : Controller
    {
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Color
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var mauSacs = db.MauSacs.ToList().ToPagedList(pageNum, pageSize);
            return View(mauSacs);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Color/Create
        public ActionResult Create()
        {
            return View();
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/Color/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MauSac mauSac)
        {
            if (ModelState.IsValid)
            {
                db.MauSacs.Add(mauSac);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mauSac);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Color/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var mauSac = db.MauSacs.Find(id);
            if (mauSac == null)
                return HttpNotFound();

            return View(mauSac);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/Color/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MauSac mauSac)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mauSac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mauSac);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Color/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var mauSac = db.MauSacs.Find(id);
            if (mauSac == null)
                return HttpNotFound();

            return View(mauSac);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/Color/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var mauSac = db.MauSacs.Find(id);
            if (mauSac != null)
            {
                db.MauSacs.Remove(mauSac);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}