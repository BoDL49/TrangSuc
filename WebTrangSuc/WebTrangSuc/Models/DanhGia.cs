//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTrangSuc.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DanhGia
    {
        public int ID { get; set; }
        public Nullable<int> IDSanPham { get; set; }
        public int IDUser { get; set; }
        public Nullable<int> Rate { get; set; }
        public string NoiDung { get; set; }
        public string TenNguoiDanhGia { get; set; }
        public System.DateTime NgayDanhGia { get; set; }
        public virtual SanPham SanPham { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
