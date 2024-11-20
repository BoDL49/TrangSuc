using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class ChatLieuController : Controller
    {
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/ChatLieu
        public ActionResult Index()
        {
            var chatLieus = db.ChatLieux.ToList();
            return View(chatLieus);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/ChatLieu/Create
        public ActionResult Create()
        {
            return View();
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/ChatLieu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChatLieu chatLieu)
        {
            if (ModelState.IsValid)
            {
                db.ChatLieux.Add(chatLieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatLieu);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/ChatLieu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var chatLieu = db.ChatLieux.Find(id);
            if (chatLieu == null)
                return HttpNotFound();

            return View(chatLieu);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/ChatLieu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChatLieu chatLieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatLieu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatLieu);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/ChatLieu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var chatLieu = db.ChatLieux.Find(id);
            if (chatLieu == null)
                return HttpNotFound();

            return View(chatLieu);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // POST: Admin/ChatLieu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var chatLieu = db.ChatLieux.Find(id);
            if (chatLieu != null)
            {
                db.ChatLieux.Remove(chatLieu);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}