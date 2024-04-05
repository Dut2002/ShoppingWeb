using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.CartRepo
{
    public class CartRepository : ICartRepository
    {
        public void AddToCart(int productId, int quantity, decimal price) => CartDAO.Instance.AddToCart(productId, quantity, price);

        public Dictionary<int, CartProduct> GetCart() => CartDAO.Instance.GetCart();

        public void RemoveFromCart(int productId) => CartDAO.Instance.RemoveFromCart(productId);

        public void UpdateCart(int productId, int quantity, decimal price) => CartDAO.Instance.UpdateCart(productId, quantity, price);
        public void DeleteCart() => CartDAO.Instance.DeleteCart();
    }
}
