using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class ReviewController : Controller
    {
        private readonly shoptrangsucEntities1 db;

        public ReviewController()
        {
            db = new shoptrangsucEntities1();
        }

        // Hiển thị danh sách sản phẩm
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult ProductList(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var products = db.SanPhams.ToList().ToPagedList(pageNum, pageSize);
            return View(products);
        }

        // Hiển thị danh sách đánh giá của sản phẩm
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int productId,int?page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var product = db.SanPhams.Find(productId);
                if (product == null)
                {
                    return HttpNotFound();
                }

                var reviews = db.DanhGias
            .Where(r => r.IDSanPham == productId)
                    .ToList().ToPagedList(pageNum, pageSize);

            ViewBag.ProductName = product.TenSanPham;
                ViewBag.ProductId = productId;

                return View(reviews);

        }
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create(int productId)
        {
            var product = db.SanPhams.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            var review = new DanhGia
            {
                IDSanPham = productId
            };

            ViewBag.ProductName = product.TenSanPham;
            ViewBag.ProductId = productId;

            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create(DanhGia review)
        {
            if (ModelState.IsValid)
            {
                db.DanhGias.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", new { productId = review.IDSanPham });
            }

            var product = db.SanPhams.Find(review.IDSanPham);
            if (product != null)
            {
                ViewBag.ProductName = product.TenSanPham;
                ViewBag.ProductId = review.IDSanPham;
            }

            return View(review);
        }


        // Sửa đánh giá (GET)
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var review = db.DanhGias.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // Sửa đánh giá (POST)
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        [HttpPost]
        public ActionResult Edit(DanhGia danhGia)
        {
            if (ModelState.IsValid)
            {
                var existingReview = db.DanhGias.Find(danhGia.ID);
                if (existingReview != null)
                {
                    existingReview.Rate = danhGia.Rate;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { productId = existingReview.IDSanPham });
                }
            }
            return View(danhGia);
        }

        // Xóa đánh giá
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Delete(int id)
        {
            var review = db.DanhGias.Find(id);
            if (review != null)
            {
                int productId = review.IDSanPham ?? 0; // Lưu lại productId trước khi xóa
                db.DanhGias.Remove(review);
                db.SaveChanges();
                return RedirectToAction("Index", new { productId });
            }
            return HttpNotFound();
        }
    }
}