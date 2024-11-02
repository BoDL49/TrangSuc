using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTrangSuc.Models; 
using System.Security.Cryptography;
using System.Text;

namespace WebTrangSuc.Controllers
{
    public class AuthController : Controller
    {
        private shoptrangsucEntities1 db = new shoptrangsucEntities1();

        public ActionResult Index()
        {
            return View();
        }


    }
}