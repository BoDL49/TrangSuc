using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.QuanLy.Controllers
{
    public class ChatLieuController : Controller
    {
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        // GET: QuanLy/ChatLieu
        [RoleAuthorization(1, 2)]
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var chatLieus = db.ChatLieux.ToList().ToPagedList(pageNum, pageSize);
            return View(chatLieus);
        }

        public ActionResult Create()
        {
            return View();
        }

        [RoleAuthorization(1, 2)]
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

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var chatLieu = db.ChatLieux.Find(id);
            if (chatLieu == null)
                return HttpNotFound();

            return View(chatLieu);
        }

        [RoleAuthorization(1, 2)]
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

        [RoleAuthorization(1, 2)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var chatLieu = db.ChatLieux.Find(id);
            if (chatLieu == null)
                return HttpNotFound();

            return View(chatLieu);
        }

        [RoleAuthorization(1, 2)] 
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