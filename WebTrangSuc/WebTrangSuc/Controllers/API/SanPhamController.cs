using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            if(danhmuc == null)
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



    }
}
