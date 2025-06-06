﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.QuanLy.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/SanPham
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Index(int? page, string SearchString = "")
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            // Lấy danh sách sản phẩm
            var sanPham = db.SanPhams.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                sanPham = sanPham.Where(s => s.TenSanPham.Contains(SearchString) || s.LoaiSanPham.TenLoaiSanPham.Contains(SearchString));
            }

            // Phân trang và chuyển dữ liệu sang View
            return View(sanPham.OrderBy(s => s.ID).ToPagedList(pageNum, pageSize));
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create()
        {
            var loaiSanPhams = db.LoaiSanPhams.ToList();
            var mauSacs = db.MauSacs.ToList();
            var chatLieux = db.ChatLieux.ToList();
            var thuongHieux = db.ThuongHieux.ToList();


            // Tạo SelectList cho dropdown
            ViewBag.LoaiSanPhamList = new SelectList(loaiSanPhams, "ID", "TenLoaiSanPham");
            ViewBag.MauList = new SelectList(mauSacs, "ID", "TenMau");
            ViewBag.ChatLieuList = new SelectList(chatLieux, "ID", "TenChatLieu");
            ViewBag.ThuongHieuList = new SelectList(thuongHieux, "ID", "TenThuongHieu");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Create(SanPham sanPham, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                // Lưu sản phẩm vào cơ sở dữ liệu
                sanPham.NgayTaoSanPham = DateTime.Now;
                db.SanPhams.Add(sanPham);
                db.SaveChanges();

                // Lưu hình ảnh sản phẩm nếu có
                if (files != null && files.Any(f => f != null && f.ContentLength > 0))
                {
                    foreach (var image in files)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(image.FileName);
                            string path = Path.Combine(Server.MapPath("~/img/"), fileName);

                            // Lưu file vào thư mục trên server
                            image.SaveAs(path);

                            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                            var hinhSanPham = new HinhSanPham
                            {
                                IDSP = sanPham.ID,
                                HinhSP = "/img/" + fileName
                            };

                            db.HinhSanPhams.Add(hinhSanPham);
                        }
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(sanPham);
        }
        // GET: Admin/SanPham/Edit/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Edit(int id)
        {
            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Truyền danh sách loại sản phẩm vào ViewBag để hiển thị trong Dropdown
            ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams, "ID", "TenLoaiSanPham", sanPham.IDLoaiSanPham);
            ViewBag.MauList = new SelectList(db.MauSacs, "ID", "TenMau", sanPham.IDMauSac);
            ViewBag.ChatLieuList = new SelectList(db.ChatLieux, "ID", "TenChatLieu", sanPham.IDChatLieu);
            ViewBag.ThuongHieuList = new SelectList(db.ThuongHieux, "ID", "TenThuongHieu", sanPham.IDThuongHieu);

            return View(sanPham);
        }

        // POST: Admin/SanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Edit(SanPham model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var sanPham = db.SanPhams.Find(model.ID);
                if (sanPham == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin sản phẩm
                sanPham.TenSanPham = model.TenSanPham;
                sanPham.Gia = model.Gia;
                sanPham.MoTaSanPham = model.MoTaSanPham;
                sanPham.SoLuongTonKho = model.SoLuongTonKho;
                sanPham.TrangThaiSanPham = model.TrangThaiSanPham;
                sanPham.IDLoaiSanPham = model.IDLoaiSanPham;
                sanPham.IDMauSac = model.IDMauSac;
                sanPham.IDChatLieu = model.IDChatLieu;
                sanPham.IDThuongHieu = model.IDThuongHieu;

                // Xóa các hình ảnh cũ nếu có
                var existingImages = db.HinhSanPhams.Where(h => h.IDSP == sanPham.ID).ToList();
                foreach (var image in existingImages)
                {
                    // Xóa file trong thư mục
                    string filePath = Server.MapPath("~" + image.HinhSP);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Xóa bản ghi trong cơ sở dữ liệu
                    db.HinhSanPhams.Remove(image);
                }

                // Lưu lại các hình ảnh mới
                if (files != null && files.Any(f => f != null && f.ContentLength > 0))
                {
                    foreach (var image in files)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(image.FileName);
                            string path = Path.Combine(Server.MapPath("~/img/"), fileName);

                            // Lưu file vào thư mục trên server
                            image.SaveAs(path);

                            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                            var hinhSanPham = new HinhSanPham
                            {
                                IDSP = sanPham.ID,
                                HinhSP = "/img/" + fileName
                            };

                            db.HinhSanPhams.Add(hinhSanPham);
                        }
                    }
                    db.SaveChanges();
                }

                // Lưu lại thông tin sản phẩm vào cơ sở dữ liệu
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu có lỗi trong model, giữ lại danh sách loại sản phẩm trong ViewBag
            ViewBag.LoaiSanPhamList = new SelectList(db.LoaiSanPhams, "ID", "TenLoaiSanPham", model.IDLoaiSanPham);
            ViewBag.MauList = new SelectList(db.MauSacs, "ID", "TenMau", model.IDMauSac);
            ViewBag.ChatLieuList = new SelectList(db.ChatLieux, "ID", "TenChatLieu", model.IDChatLieu);
            ViewBag.ThuongHieuList = new SelectList(db.ThuongHieux, "ID", "TenThuongHieu", model.IDThuongHieu);
            return View(model);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Delete(int id)
        {
            // Lấy sản phẩm từ database
            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy sản phẩm
            }

            return View(sanPham); // Trả về view xác nhận xóa
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult DeleteConfirmed(int id)
        {
            var sanPham = db.SanPhams.Include(s => s.HinhSanPhams).FirstOrDefault(s => s.ID == id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Delete associated images first
            foreach (var hinh in sanPham.HinhSanPhams.ToList())
            {
                db.HinhSanPhams.Remove(hinh);
            }

            // Now delete the product
            db.SanPhams.Remove(sanPham);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}