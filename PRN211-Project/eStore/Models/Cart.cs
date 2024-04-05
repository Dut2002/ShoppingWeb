using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eStore.Models
{
    public class Cart
    {
        private static Dictionary<int, int> cart;
        //private Cart()
        //{

        //}

        //// Using Singleton Pattern
        //private static Cart instance = null;
        //private static object instanceLock = new object();

        //public static Cart Instance
        //{
        //    get
        //    {
        //        lock(instanceLock)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new Cart();
        //            }
        //            return instance;
        //        }
        //    }
        //}

        public Dictionary<int, int> GetCart() => cart;
        public void AddToCart(int productId, int quantity)
        {
            // Check existed cart
            if (cart == null)
            {
                cart = new Dictionary<int, int>();
            }
            // Check existed Product
            if (cart.ContainsKey(productId))
            {
                int _quantity;
                cart.TryGetValue(productId, out _quantity);
                quantity = _quantity + quantity;

                cart[productId] = quantity;
            } else
            {
                // Update cart
                cart.Add(productId, quantity);
            }
            
            
        }
        public void RemoveFromCart(int ProductId)
        {
            // Check existed cart
            if (cart != null)
            {
                // Check existed book
                if (cart.ContainsKey(ProductId))
                {
                    cart.Remove(ProductId);
                    if (cart.Count == 0)
                    {
                        cart = null;
                    }
                }
            }
            
        }
        public void UpdateCart(int productId, int quantity)
        {
            if (cart != null)
            {
                if (cart.ContainsKey(productId))
                {
                    cart[productId] = quantity;
                }
            }
        }

        public void DeleteCart()
        {
            cart = null;
        }
    }
}
