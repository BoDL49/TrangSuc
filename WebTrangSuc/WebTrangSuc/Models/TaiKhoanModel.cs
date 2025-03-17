using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebTrangSuc.Models
{
    public class TaiKhoanModel
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NamSinh { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(10, ErrorMessage = "Số điện thoại không hợp lệ.", MinimumLength = 10)]
        public string SDT { get; set; }

        //[Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        //[EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(100, ErrorMessage = "Tên đăng nhập tối đa 100 ký tự và tối thiểu là 3.", MinimumLength = 3)]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        public string Matkhau { get; set; }

        public string Avatar { get; set; }

        public int IDRole { get; set; }

        public virtual Role Role { get; set; }

        [NotMapped]
        public string XacNhanMatKhau { get; set; }

        [NotMapped]
        public string Diachi { get; set; }
    }
}