using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTrangSuc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ChiTietSanPham",
                url: "chitietsanpham/{slug}",
                defaults: new { controller = "ChiTietSanPham", action = "Index", slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SanPhamApi",
                url: "api/sanpham/byname/{slug}",
                defaults: new { controller = "SanPham", action = "LaySanPhamTheoTen", slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "TrangChu", action = "Index", id = UrlParameter.Optional }
            );
        }

    }
}
