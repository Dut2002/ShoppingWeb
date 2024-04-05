using BusinessObject;
using DataAccess.Repository.MemberRepo;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.OrderRepo;
using DataAccess.Repository.ProductRepo;
using eStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eStore.Controllers
{
    public class CartController : Controller
    {
        IProductRepository productRepository = null;

        public CartController()
        {
            productRepository = new ProductRepository();
        }

        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            try
            {
                IEnumerable<CartItem> cartItems = new List<CartItem>();

                Cart cart = HttpContext.Session.GetComplexData<Cart>("CART");

                if (cart != null)
                {
                    var cartDictionary = cart.GetCart();
                    if (cartDictionary != null)
                    {
                        var products = cartDictionary.Keys;

                        foreach (var product in products)
                        {
                            Product _product = productRepository.GetProduct(product);
                            cartItems = cartItems.Append(new CartItem
                            {
                                ProductId = _product.ProductId,
                                ProductName = _product.ProductName,
                                Quantity = cartDictionary.GetValueOrDefault(product, 0),
                                Price = _product.UnitPrice,
                                Total = _product.UnitPrice * cartDictionary.GetValueOrDefault(product, 0)
                            });
                        }
                    }
                    
                }

                return View(cartItems);
            } catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult AddToCart(int? productId, int? quantity)
        {
            try
            {
                if (productId == null)
                {
                    throw new Exception("Product is not existed!!");
                }
                if (quantity == null)
                {
                    throw new Exception("Please enter the quantity at least 1!");
                }

                Product product = productRepository.GetProduct(productId.Value);

                if (product == null)
                {
                    throw new Exception("Product is not existed!!");
                }

                Cart cart = HttpContext.Session.GetComplexData<Cart>("CART");

                if (cart == null)
                {
                    cart = new Cart();
                }

                cart.AddToCart(productId.Value, quantity.Value);

                HttpContext.Session.SetComplexData("CART", cart);
                TempData["AddSuccess"] = "Add to Cart successfully!! Click <a href='/Cart'>here</a> to view your cart!";
                return RedirectToAction("Details", "Product", new { id = productId });

            }
            catch (Exception ex)
            {
                TempData["AddError"] = ex.Message;
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }

        [Authorize(Roles = "User")]
        public IActionResult RemoveFromCart(int productId)
        {
            Cart cart = HttpContext.Session.GetComplexData<Cart>("CART");

            if (cart != null)
            {
                cart.RemoveFromCart(productId);
            }

            HttpContext.Session.SetComplexData("CART", cart);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            Cart cart = HttpContext.Session.GetComplexData<Cart>("CART");

            if (cart != null)
            {
                cart.UpdateCart(productId, quantity);
            }

            HttpContext.Session.SetComplexData("Cart", cart);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public IActionResult Order()
        {
            Cart cart = HttpContext.Session.GetComplexData<Cart>("CART");

            if (cart != null)
            {
                var cartDiction = cart.GetCart();

                IOrderDetailRepository orderDetailRepository = new OrderDetailRepository();
                IEnumerable<OrderDetail> orderItems = new List<OrderDetail>();

                Order order = new Order();

                string memberId = User.Claims.FirstOrDefault(c => c.Type.Equals("MemberId")).Value;
                IMemberRepository memberRepository = new MemberRepository();
                Member loginMember = memberRepository.GetMember(int.Parse(memberId));

                DateTime dateTime = DateTime.Today;

                if (cartDiction != null && cartDiction.Count > 0)
                {
                    try
                    {
                        order = new Order
                        {
                            MemberId = loginMember.MemberId,
                            OrderDate = dateTime
                        };

                        IOrderRepository orderRepository = new OrderRepository();

                        Order insertedOrder = orderRepository.AddOrder(order);

                        foreach (var cartItem in cartDiction)
                        {
                            decimal unitPrice = productRepository.GetProduct(cartItem.Key).UnitPrice;
                            decimal discount = 0;

                            OrderDetail orderDetail = new OrderDetail()
                            {
                                OrderId = insertedOrder.OrderId,
                                ProductId = cartItem.Key,
                                UnitPrice = unitPrice,
                                Quantity = cartItem.Value,
                                Discount = Convert.ToDouble(discount)
                            };

                            orderDetailRepository.AddOrderDetail(orderDetail);
                            Product product = productRepository.GetProduct(cartItem.Key);
                            product.UnitsInStock = product.UnitsInStock - cartItem.Value;
                            productRepository.Update(product);

                        }

                        HttpContext.Session.SetComplexData("CART", null);
                        ViewBag.Success = "Check out successfully!! Click <a href='/Product'>here</a> to continue shopping.";
                        return View();
                    } catch (Exception ex)
                    {
                        ViewBag.Error = ex.Message;
                        return View();
                    }
                } else
                {
                    ViewBag.Error = "Your Cart is empty!!";
                }
            } else 
            {
                ViewBag.Error = "Your Cart is empty!!";
            }
            return View();
        }
    }
}
