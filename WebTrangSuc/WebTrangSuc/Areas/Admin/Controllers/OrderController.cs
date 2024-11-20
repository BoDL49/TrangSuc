using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Order
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            // Lấy danh sách đơn hàng cùng thông tin khách hàng và địa chỉ
            var orders = db.DonHangs
                .Include("TaiKhoan")
                .Include("TaiKhoan.DiaChis")
                .ToList().ToPagedList(pageNum, pageSize);

            return View(orders);
        }

        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Order/Details/5
        public ActionResult Details(int id)
        {
            var order = db.DonHangs
                .Include("DonHangChiTiets")
                .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Admin/Order/UpdateDelivery/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult UpdateDelivery(int id, int updateDelivery)
        {
            var order = db.DonHangs.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.TrangThaiGiaoHang = updateDelivery;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Admin/Order/Delete/5
        // GET: Admin/Order/Delete/5
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        public ActionResult Delete(int id)
        {
            // Lấy thông tin đơn hàng và bao gồm DonHangChiTiets
            var order = db.DonHangs
                            .Where(o => o.ID == id)
                            .Include(o => o.DonHangChiTiets) // Eager load các chi tiết đơn hàng
                            .FirstOrDefault();

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            // Tiến hành xóa đơn hàng
            db.DonHangs.Remove(order);
            db.SaveChanges();
            TempData["Success"] = "Order deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}