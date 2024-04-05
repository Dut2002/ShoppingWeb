using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ProductDAO
    {
        // Singleton
        private static ProductDAO instance;
        private static object instanceLock = new object();

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Product> GetProductsList(bool order = false)
        {
            IEnumerable<Product> products = null;

            try
            {
                var context = new SalesManagementContext();
                // Get From Database
                if (order)
                {
                    // Get Units In Stock > 0
                    products = context.Products
                            .Where(pro => pro.UnitsInStock > 0)
                           .Include(pro => pro.Category).ToList();
                } else
                {
                    products = context.Products
                            .Include(pro => pro.Category).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return products;
        }

        public Product GetProduct(int productId, IEnumerable<Product> searchList = null)
        {
            Product product = null;

            try
            {
                if (searchList == null)
                {
                    var context = new SalesManagementContext();
                    var _ = context.Products.Where(pro => pro.ProductId == productId)
                        .Include(pro => pro.Category);
                    if (_ != null && _.Any())
                    {
                        product = _.First();
                    }
                } else
                {
                    product = searchList.Where(pro => pro.ProductId == productId)
                        .AsQueryable()
                        .Include(pro => pro.Category).First();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return product;
        }

        public Product GetProduct(string productName, IEnumerable<Product> searchList = null)
        {
            Product product = null;

            try
            {
                if (searchList == null)
                {
                    var context = new SalesManagementContext();
                    var _ = context.Products.Where(pro => pro.ProductName.Equals(productName))
                        .Include(pro => pro.Category);
                    if (_ != null && _.Any())
                    {
                        product = _.First();
                    }
                }
                else
                {
                    product = searchList.Where(pro => pro.ProductName.Equals(productName))
                        .AsQueryable()
                        .Include(pro => pro.Category).First();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return product;
        }

        public int GetNextProductId()
        {
            int nextMemberId = -1;

            try
            {
                var context = new SalesManagementContext();
                nextMemberId = context.Products.Max(pro => pro.ProductId) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return nextMemberId;
        }
        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new Exception("Product is undefined!!");
            }
            try
            {
                if (GetProduct(product.ProductId) == null)
                {
                    var context = new SalesManagementContext();
                    context.Products.Add(product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product is existed!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(Product product)
        {
            if (product == null)
            {
                throw new Exception("Product is undefined!!");
            }
            try
            {
                Product _mem = GetProduct(product.ProductId);
                if (_mem != null)
                {
                    var context = new SalesManagementContext();
                    context.Products.Update(product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void Delete(int productId)
        {
            try
            {
                Product Product = GetProduct(productId);
                if (Product != null)
                {
                    var context = new SalesManagementContext();
                    context.Products.Remove(Product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Product does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Product> SearchProduct(string name, IEnumerable<Product> searchList)
        {
            IEnumerable<Product> searchResult = null;

            try
            {
                if (searchList == null)
                {
                    var context = new SalesManagementContext();
                    searchResult = context.Products
                                        .Where(pro => pro.ProductName.ToLower().Contains(name.ToLower()))
                                        .Include(pro => pro.Category).ToList();
                } else
                {
                    searchResult = searchList.Where(pro => pro.ProductName.ToLower().Contains(name.ToLower())).ToList();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchResult;
        }

        public IEnumerable<Product> SearchProduct(int startUnit, int endUnit, IEnumerable<Product> searchList = null)
        {
            IEnumerable<Product> searchResult = null;

            try
            {
                if (searchList == null)
                {
                    var context = new SalesManagementContext();
                    searchResult = context.Products
                                    .Where(pro => pro.UnitsInStock >= startUnit && pro.UnitsInStock <= endUnit)
                                    .Include(pro => pro.Category).ToList();
                } else
                {
                    searchResult = searchList.Where(pro => pro.UnitsInStock >= startUnit && pro.UnitsInStock <= endUnit).ToList();
                }
                

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return searchResult;
        }

        public IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice, IEnumerable<Product> searchList = null)
        {
            IEnumerable<Product> searchResult = null;

            try
            {
                if (searchList == null)
                {
                    var context = new SalesManagementContext();
                    searchResult = context.Products
                                        .Where(pro => pro.UnitPrice >= startPrice && pro.UnitPrice <= endPrice)
                                        .Include(pro => pro.Category).ToList();
                } else
                {
                    searchResult = searchList.Where(pro => pro.UnitPrice >= startPrice && pro.UnitPrice <= endPrice).ToList();
                }
                
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return searchResult;
        }
    }
}
