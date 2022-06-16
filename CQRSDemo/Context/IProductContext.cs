using CQRSDemo.Models;

namespace CQRSDemo.Context
{
    public interface IProductContext
    {
        Task<int> Add(Product product);
        Task<bool> Remove(int id);
        Task<bool> Update(Product product);
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(int id);
    }
}
