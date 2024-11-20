using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTrangSuc.Models
{
    public class SanPhamGioHangModel
    {
        public int ID { get; set; }
        public int IDUser { get; set; }
        public int IDSanPham { get; set; }
        public int SoLuongSanPham { get; set; }
        public int Gia { get; set; }

        public TaiKhoan TaiKhoan { get; set; }
        public SanPham SanPham { get; set; }
    }

    public class AddToCartDto
    {
        public int IDUser { get; set; }
        public int IDSanPham { get; set; }
        public int SoLuongSanPham { get; set; }
        public int Gia { get; set; }
    }

    public class UpdateCartDto
    {
        public int ID { get; set; }
        public int SoLuongSanPham { get; set; }
        public int Gia { get; set; }
    }

    public class ApplyVoucherDto
    {
        public int UserId { get; set; }
        public string VoucherCode { get; set; }
    }

}