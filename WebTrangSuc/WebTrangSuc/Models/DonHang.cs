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
    
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            this.DonHangChiTiets = new HashSet<DonHangChiTiet>();
        }
    
        public int ID { get; set; }
        public Nullable<int> IDUser { get; set; }
        public Nullable<int> TongTien { get; set; }
        public Nullable<System.DateTime> NgayTaoHoaDon { get; set; }
        public Nullable<int> TrangThaiGiaoHang { get; set; }
        public Nullable<int> TrangThaiDonHang { get; set; }
        public Nullable<int> IDVoucher { get; set; }
        public string PhuongThucThanhToan { get; set; }
    
        public virtual TaiKhoan TaiKhoan { get; set; }
        public virtual Voucher Voucher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }
    }
}
