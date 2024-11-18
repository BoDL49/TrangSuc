using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebTrangSuc.Models
{
    public class SanPhamModel
    {
        public int ID { get; set; }
        public string TenSanPham { get; set; }
        public int Gia { get; set; }
        public string MoTaSanPham { get; set; }

        public int IDLoaiSanPham { get; set; }
        [NotMapped]
        public string TenLoaiSanPham { get; set; }

        public int SoLuongTonKho { get; set; }
        public string TrangThaiSanPham { get; set; }
        public DateTime NgayTaoSanPham { get; set; }

        public int IDChatLieu { get; set; }
        [NotMapped]
        public string TenChatLieu { get; set; }

        public int IDMauSac { get; set; }
        [NotMapped]
        public string TenMau { get; set; }

        public int IDThuongHieu { get; set; }
        [NotMapped]
        public string TenThuongHieu { get; set; }

        public string IDHinhSanPham { get; set; }
        public string HinhSanPham { get; set; }

        public string IDDanhGia { get; set; }
        public int Rate { get; set; }

        public virtual ChatLieu ChatLieu { get; set; }
        public virtual MauSac MauSac { get; set; }
        public virtual ThuongHieu ThuongHieu { get; set; }
    }
}