using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTrangSuc.Models
{
    public class CancelledOrderState : IOrderState
    {
        public void ProcessOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã hủy, không thể xử lý.");
        public void CancelOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã hủy rồi.");
        public void DeliverOrder(DonHang order) => throw new InvalidOperationException("Đơn hàng đã hủy, không thể giao.");
        public string GetStatus() => "Hủy";
    }
}
