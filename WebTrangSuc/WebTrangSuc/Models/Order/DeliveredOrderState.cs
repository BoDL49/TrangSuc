using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTrangSuc.Models
{
    public class DeliveredOrderState : IOrderState
    {
        public void ProcessOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã giao, không thể xử lý lại.");
        public void CancelOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã giao, không thể hủy.");
        public void DeliverOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã được giao rồi.");
        public string GetStatus() => "Đã giao";
    }
}
