using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class ChiTietSanPhamController : Controller
    {
        private readonly shoptrangsucEntities1 _context = new shoptrangsucEntities1();

        // GET: ChiTietSanPham
        public ActionResult Index(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Index", "DanhMucSanPham");
            }

            ViewBag.Slug = slug;
            return View();

        }
    }
}