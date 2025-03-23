using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTrangSuc.Models
{
    public class NewOrderState : IOrderState
    {
        public void ProcessOrder(DonHang order)
        {
            order.TrangThaiDonHang = 1;
            order.OrderState = new ProcessingOrderState();
        }

        public void CancelOrder(DonHang order)
        {
            order.TrangThaiDonHang = 3;
            order.OrderState = new CancelledOrderState();
        }

        public void DeliverOrder(DonHang order)
        {
            throw new InvalidOperationException("Đơn hàng mới không thể giao ngay.");
        }

        public string GetStatus() => "Mới";
    }
}
