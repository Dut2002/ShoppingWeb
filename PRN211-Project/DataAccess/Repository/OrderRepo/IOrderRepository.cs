using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.OrderRepo
{
    public interface IOrderRepository
    {
        public Order AddOrder(Order order);
        public IEnumerable<Order> GetOrders(int memberId);
        public IEnumerable<Order> GetOrders(int memberId, DateTime startDate, DateTime endDate);
        public Order GetOrder(int orderId);
        public void DeleteOrder(int orderId);
        public void DeleteByMember(int memberId);
    }
}
