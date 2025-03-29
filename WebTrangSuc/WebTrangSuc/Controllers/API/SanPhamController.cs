using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using WebTrangSuc.Models;

namespace WebTrangSuc.Controllers
{
    public class SanPhamController : ApiController
    {
        private readonly shoptrangsucEntities1 _context;

        public SanPhamController()
        {
            _context = new shoptrangsucEntities1();
        }

        public SanPhamController(shoptrangsucEntities1 context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/sanpham")]
        public async Task<IHttpActionResult> LaySanPham()
        {
            var sanPham = _context.SanPhams.Select(sp => new SanPhamModel
            {
                ID = sp.ID,
                TenSanPham = sp.TenSanPham,
                Gia = (int)sp.Gia,
                MoTaSanPham = sp.MoTaSanPham,
                IDLoaiSanPham = (int)sp.IDLoaiSanPham,
                SoLuongTonKho = (int)sp.SoLuongTonKho,
                TenLoaiSanPham = sp.LoaiSanPham.TenLoaiSanPham,
                TrangThaiSanPham = sp.TrangThaiSanPham,
                NgayTaoSanPham = (DateTime)sp.NgayTaoSanPham,
                TenChatLieu = sp.ChatLieu.TenChatLieu,
                TenMau = sp.MauSac.TenMau,
                TenThuongHieu = sp.ThuongHieu.TenThuongHieu,
                HinhSanPham = sp.HinhSanPhams.FirstOrDefault().HinhSP,
            });
            return Ok(sanPham);
        }

        [HttpGet]
        [Route("api/sanpham/{id}")]
        public async Task<IHttpActionResult> LaySanPhamChiTiet(int id)
        {
            var sanpham = _context.SanPhams.Where(sp => sp.ID == id).Select(sp => new SanPhamModel
            {
                ID = sp.ID,
                TenSanPham = sp.TenSanPham,
                Gia = (int)sp.Gia,
                MoTaSanPham = sp.MoTaSanPham,
                TenChatLieu = sp.ChatLieu.TenChatLieu,
                TenMau = sp.MauSac.TenMau,
                TenThuongHieu = sp.ThuongHieu.TenThuongHieu
            }).FirstOrDefault();

            var HinhSanpham = _context.HinhSanPhams.Where(hsp => hsp.IDSP == id).Select(hsp => new HinhSanPhamModel
            {
                ID = hsp.ID,
                IDSP = (int)hsp.IDSP,
                HinhSP = hsp.HinhSP
            }).ToList();

            if (sanpham == null)
            {
                return NotFound();
            }

            return Ok(new { sanpham, HinhSanpham });
        }

        [HttpGet]
        [Route("api/sanpham/loaisanpham/")]
        public async Task<IHttpActionResult> LayDanhMucSP()
        {
            var danhmuc = _context.LoaiSanPhams.Select(sp => new LoaiSanPhamModel
            {
                ID = sp.ID,
                TenLoaiSanPham = sp.TenLoaiSanPham
            }).ToList();

            if (danhmuc == null)
            {
                return NotFound();
            }

            return Ok(danhmuc);
        }

        [HttpGet]
        [Route("api/sanpham/loaisanpham/{id}")]
        public async Task<IHttpActionResult> LaySanPhamTheoLoai(int id)
        {
            var sanPhams = _context.SanPhams
                .Where(sp => sp.IDLoaiSanPham == id)
                .Select(sp => new SanPhamModel
                {
                    ID = sp.ID,
                    TenSanPham = sp.TenSanPham,
                    Gia = (int)sp.Gia,
                    MoTaSanPham = sp.MoTaSanPham,
                    IDLoaiSanPham = (int)sp.IDLoaiSanPham,
                    HinhSanPham = sp.HinhSanPhams.FirstOrDefault().HinhSP,
                }).ToList();

            if (!sanPhams.Any())
            {
                return NotFound();
            }

            return Ok(sanPhams);
        }

        [HttpGet]
        [Route("api/sanpham/search/{query}")]
        public IHttpActionResult SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var products = _context.SanPhams
                .Where(sp => sp.TenSanPham.Contains(query))
                .Select(sp => new
                {
                    sp.ID,
                    sp.TenSanPham,
                    sp.Gia,
                    sp.MoTaSanPham,
                    HinhSanPham = sp.HinhSanPhams.FirstOrDefault().HinhSP
                }).ToList();

            return Ok(products);
        }


        [HttpGet]
        [Route("api/sanpham/byname/{slug}")]
        public async Task<IHttpActionResult> LaySanPhamTheoTen(string slug)
        {
            try
            {
                // Hàm chuẩn hóa tên sản phẩm để loại bỏ dấu nhưng giữ nguyên "Đ" hoặc chuyển đổi phù hợp
                string NormalizeSlug(string text)
                {
                    if (string.IsNullOrEmpty(text)) return text;
                    string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
                    // Chỉ loại bỏ các dấu phụ, giữ nguyên ký tự gốc như "Đ"
                    string result = Regex.Replace(normalizedString, @"[\u0300-\u036f]", "")
                                         .Replace("Đ", "D") // Chuyển "Đ" thành "D"
                                         .Replace("đ", "d") // Chuyển "đ" thành "d"
                                         .ToLower()
                                         .Replace(" ", "-")
                                         .Replace("--", "-"); // Xử lý nhiều dấu gạch liên tiếp
                    return result;
                }

                // Lấy tất cả sản phẩm và lọc sau khi lấy ra khỏi cơ sở dữ liệu
                var allSanPhams = await _context.SanPhams.ToListAsync();
                var sanpham = allSanPhams
                    .FirstOrDefault(sp => NormalizeSlug(sp.TenSanPham) == slug.ToLower());

                if (sanpham == null)
                {
                    return NotFound();
                }

                // Lấy danh sách hình ảnh sản phẩm
                var hinhSanpham = await _context.HinhSanPhams
                    .Where(hsp => hsp.IDSP == sanpham.ID)
                    .Select(hsp => new HinhSanPhamModel
                    {
                        ID = hsp.ID,
                        IDSP = (int)hsp.IDSP,
                        HinhSP = hsp.HinhSP
                    })
                    .ToListAsync();

                var sanPhamModel = new SanPhamModel
                {
                    ID = sanpham.ID,
                    TenSanPham = sanpham.TenSanPham,
                    Gia = (int)sanpham.Gia,
                    MoTaSanPham = sanpham.MoTaSanPham,
                    TenChatLieu = sanpham.ChatLieu.TenChatLieu,
                    TenMau = sanpham.MauSac.TenMau,
                    TenThuongHieu = sanpham.ThuongHieu.TenThuongHieu
                };

                return Ok(new { sanpham = sanPhamModel, HinhSanpham = hinhSanpham });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



    }
}
