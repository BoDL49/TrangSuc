using System.Web.Mvc;

namespace WebTrangSuc.Areas.XuLyDonHang
{
    public class XuLyDonHangAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "XuLyDonHang";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "XuLyDonHang_default",
                "XuLyDonHang/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}