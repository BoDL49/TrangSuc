using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTrangSuc.Models
{
    public class ProcessingOrderState : IOrderState
    {
        public void ProcessOrder(DonHang order)
        {
            throw new InvalidOperationException("Đơn hàng đang được xử lý rồi.");
        }

        public void CancelOrder(DonHang order)
        {
            order.TrangThaiDonHang = 3;
            order.OrderState = new CancelledOrderState();
        }

        public void DeliverOrder(DonHang order)
        {
            order.TrangThaiDonHang = 2;
            order.TrangThaiGiaoHang = 1;
            order.OrderState = new DeliveredOrderState();
        }

        public string GetStatus() => "Đang xử lý";
    }
}
