using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebTrangSuc.Models;
using WebTrangSuc.Views.JwtAuthorizeAttribute;

using WebTrangSuc.Controllers.API; 

namespace WebTrangSuc.Areas.Admin.Controllers
{
    public class ReviewController : Controller
    {
        private readonly shoptrangsucEntities1 db;
        private readonly HttpClient _httpClient;

        public ReviewController()
        {
            db = new shoptrangsucEntities1();
            _httpClient = new HttpClient();
        }

        [RoleAuthorization(1, 2, 3)] 
        public ActionResult ProductList(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var products = db.SanPhams.OrderBy(p => p.ID).ToPagedList(pageNum, pageSize);
            return View(products);
        }

        [RoleAuthorization(1, 2, 3)] 
        public async Task<ActionResult> Index(int productId, int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var product = db.SanPhams.Find(productId);

            if (product == null)
            {
                return HttpNotFound();
            }

            List<ReviewDto> reviews = new List<ReviewDto>(); 
            try
            {
                var apiUrl = $"http://localhost:55119/api/danhgia/danhsach/{productId}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    reviews = await response.Content.ReadAsAsync<List<WebTrangSuc.Controllers.API.ReviewDto>>();
                }
                else
                {
                    ViewBag.ErrorMessage = $"Error fetching reviews: {(int)response.StatusCode} - {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Error connecting to review service: {ex.Message}";
            }
            catch (Exception ex) 
            {
                ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
               
            }

            var pagedReviews = new StaticPagedList<WebTrangSuc.Controllers.API.ReviewDto>(
                reviews ?? new List<WebTrangSuc.Controllers.API.ReviewDto>(), 
                pageNum,
                pageSize,
                reviews?.Count ?? 0 
            );

            ViewBag.ProductName = product.TenSanPham;
            ViewBag.ProductId = productId;

            return View(pagedReviews);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
                _httpClient?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
