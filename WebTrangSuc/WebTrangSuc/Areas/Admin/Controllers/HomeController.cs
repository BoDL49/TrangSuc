using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [RoleAuthorization(1, 2, 3)] // Chỉ cho phép role 1, 2, 3
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}