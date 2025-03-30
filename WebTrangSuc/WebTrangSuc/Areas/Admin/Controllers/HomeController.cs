using System;
using System.Linq;
using System.Web.Mvc;
using WebTrangSuc.Models;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly shoptrangsucEntities1 db;

        public HomeController()
        {
            db = new shoptrangsucEntities1();
        }

        public ActionResult Index()
        {
            ViewBag.TotalOrders = db.DonHangs.Count();
            ViewBag.DeliveredOrders = db.DonHangs.Count(o => o.TrangThaiDonHang == 2); 
            ViewBag.NewOrders = db.DonHangs.Count(o => o.TrangThaiDonHang == 0); 
            ViewBag.CancelledOrders = db.DonHangs.Count(o => o.TrangThaiDonHang == 3); 
            ViewBag.TotalProducts = db.SanPhams.Count();
            ViewBag.TotalUsers = db.TaiKhoans.Count();
            ViewBag.TotalRevenue = db.DonHangs.Where(o => o.TrangThaiDonHang == 2).Sum(o => (decimal?)o.TongTien) ?? 0;

            var revenueData = db.DonHangs
    .Where(o => o.TrangThaiDonHang == 2)
    .GroupBy(o => new { Year = o.NgayTaoHoaDon.Value.Year, Month = o.NgayTaoHoaDon.Value.Month })
    .Select(g => new
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        Revenue = g.Sum(o => o.TongTien)
    })
    .ToList()
    .Select(g => new
    {
        Date = new DateTime(g.Year, g.Month, 1),
        g.Revenue
    })
    .OrderBy(g => g.Date)
    .ToList();

            ViewBag.RevenueDates = revenueData.Select(d => d.Date.ToString("yyyy-MM")).ToArray();
            ViewBag.RevenueValues = revenueData.Select(d => d.Revenue).ToArray();

            return View();
        }
    }
}

