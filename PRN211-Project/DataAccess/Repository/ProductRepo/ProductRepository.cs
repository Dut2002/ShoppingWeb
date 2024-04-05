using BusinessObject;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        public void AddProduct(Product product) => ProductDAO.Instance.AddProduct(product);

        public void Delete(int productId) => ProductDAO.Instance.Delete(productId);

        public int GetNextProductId()
        {
            return ProductDAO.Instance.GetNextProductId();
        }

        public Product GetProduct(int productId, IEnumerable<Product> searchList = null)
        {
            return ProductDAO.Instance.GetProduct(productId, searchList);
        }
        public Product GetProduct(string productName, IEnumerable<Product> searchList = null)
        {
            return ProductDAO.Instance.GetProduct(productName, searchList);
        }

        public IEnumerable<Product> GetProductsList(bool order = false)
        {
            return ProductDAO.Instance.GetProductsList(order);
        }

        public IEnumerable<Product> SearchProduct(string name, IEnumerable<Product> searchList = null)
        {
            return ProductDAO.Instance.SearchProduct(name, searchList);
        }

        public IEnumerable<Product> SearchProduct(int startUnit, int endUnit, IEnumerable<Product> searchList = null)
        {
            return ProductDAO.Instance.SearchProduct(startUnit, endUnit, searchList);
        }

        public IEnumerable<Product> SearchProduct(decimal startPrice, decimal endPrice, IEnumerable<Product> searchList = null)
        {
            return ProductDAO.Instance.SearchProduct(startPrice, endPrice, searchList);
        }

        public void Update(Product product) => ProductDAO.Instance.Update(product);
    }
}
