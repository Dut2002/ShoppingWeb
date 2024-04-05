using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.CategoryRepo
{
    public interface ICategoryRepository
    {
        public IEnumerable<Category> GetCategoryList();
        public Category GetCategory(int categoryId);
        public Category GetCategory(string categoryName);
        public void AddCategory(string categoryName);
    }
}
