using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.OrderDetailRepo
{
    public interface IOrderDetailRepository
    {
        public void AddOrderDetail(OrderDetail orderDetail);
        public decimal GetOrderTotal(int orderId);
        public IEnumerable<OrderDetail> GetOrderDetails(int orderId);
        public void DeleteOrderDetails(int orderId);
        public void DeleteByProduct(int productId);
    }
}
