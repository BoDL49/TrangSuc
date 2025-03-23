using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.QuanLy.Controllers
{
    public class OrderController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        [RoleAuthorization(1, 2, 3)]
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var orders = db.DonHangs
                .Include("TaiKhoan")
                .Include("TaiKhoan.DiaChis")
                .ToList()
                .ToPagedList(pageNum, pageSize);

            foreach (var order in orders)
            {
                InitializeOrderState(order);
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return View(orders);
        }

        [RoleAuthorization(1, 2, 3)]
        public ActionResult Details(int id)
        {
            var order = db.DonHangs
                .Include("DonHangChiTiets.SanPham")
                .Include("TaiKhoan")
                .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            InitializeOrderState(order);
            return View(order);
        }

        [RoleAuthorization(1, 2, 3)]
        public ActionResult Edit(int id)
        {
            var order = db.DonHangs
                .Include("DonHangChiTiets.SanPham")
                .Include("TaiKhoan")
                .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            InitializeOrderState(order);
            return View(order);
        }

        [HttpPost]
        [RoleAuthorization(1, 2, 3)]
        public ActionResult Edit(DonHang model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = db.DonHangs
                .Include(o => o.DonHangChiTiets)
                .Include(o => o.TaiKhoan)
                .FirstOrDefault(o => o.ID == model.ID);

            if (order == null)
            {
                return HttpNotFound();
            }

            try
            {
                InitializeOrderState(order);

                if (model.TrangThaiDonHang != order.TrangThaiDonHang)
                {
                    switch (model.TrangThaiDonHang)
                    {
                        case 0:
                            order.TrangThaiDonHang = 0;
                            order.OrderState = new NewOrderState();
                            break;
                        case 1:
                            order.Process();
                            break;
                        case 2:
                            order.Deliver();
                            break;
                        case 3:
                            order.Cancel();
                            break;
                        default:
                            throw new Exception("Trạng thái không hợp lệ.");
                    }
                }

                if (model.TrangThaiGiaoHang != order.TrangThaiGiaoHang)
                {
                    if (model.TrangThaiGiaoHang == 1 && order.TrangThaiDonHang == 2)
                    {
                        order.Deliver();
                    }
                    else if (model.TrangThaiGiaoHang == 0)
                    {
                        order.TrangThaiGiaoHang = 0;
                    }
                }

                db.SaveChanges();
                TempData["Success"] = "Đơn hàng đã được cập nhật.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật đơn hàng: {ex.Message}";
                InitializeOrderState(order);
                return View(order);
            }
        }




        [RoleAuthorization(1, 2, 3)]
        public ActionResult UpdateDelivery(int id, int updateDelivery)
        {
            var order = db.DonHangs.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            try
            {
                InitializeOrderState(order);
                if (updateDelivery == 1)
                {
                    order.Deliver();
                    TempData["Success"] = "Đơn hàng đã được giao.";
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật giao hàng: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [RoleAuthorization(1, 2, 3)]
        public ActionResult UpdateDonHang(int id, int updateDonHang)
        {
            var order = db.DonHangs.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            try
            {
                InitializeOrderState(order);

                switch (updateDonHang)
                {
                    case 1:
                        order.Process();
                        TempData["Success"] = "Đơn hàng đang được xử lý.";
                        break;
                    case 2:
                        if (order.TrangThaiDonHang != 1)
                        {
                            throw new InvalidOperationException("Đơn hàng phải ở trạng thái 'Đang xử lý' để có thể giao.");
                        }
                        order.Deliver();
                        TempData["Success"] = "Đơn hàng đã được giao.";
                        break;
                    case 3:
                        order.Cancel();
                        TempData["Success"] = "Đơn hàng đã bị hủy.";
                        break;
                    default:
                        TempData["Error"] = "Trạng thái không hợp lệ.";
                        break;
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật trạng thái: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        [RoleAuthorization(1, 2, 3)]
        public ActionResult Delete(int id)
        {
            var order = db.DonHangs
                .Include(o => o.DonHangChiTiets)
                .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index");
            }

            db.DonHangs.Remove(order);
            db.SaveChanges();
            TempData["Success"] = "Order deleted successfully.";
            return RedirectToAction("Index");
        }

        private void InitializeOrderState(DonHang order)
        {
            if (order.OrderState == null)
            {
                switch (order.TrangThaiDonHang)
                {
                    case 0:
                        order.OrderState = new NewOrderState();
                        break;
                    case 1:
                        order.OrderState = new ProcessingOrderState();
                        break;
                    case 2:
                        order.OrderState = new DeliveredOrderState();
                        break;
                    case 3:
                        order.OrderState = new CancelledOrderState();
                        break;
                    default:
                        order.OrderState = new NewOrderState();
                        break;
                }
            }
        }

    }
}