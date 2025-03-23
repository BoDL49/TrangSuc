using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTrangSuc.Models
{
    public interface IOrderState
    {
        void ProcessOrder(DonHang order); 
        void CancelOrder(DonHang order);  
        void DeliverOrder(DonHang order); 
        string GetStatus();               
    }
}
