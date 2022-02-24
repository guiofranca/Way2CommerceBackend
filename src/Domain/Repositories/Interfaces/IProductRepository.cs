using Domain.Models;
using Domain.Repositories.Interfaces.Shared;

namespace Domain.Repositories.Interfaces;
public interface IProductRepository : IBaseRepository<Product>
{
    //public Task<Product> LoadCategory(Product product);
    //public Task<List<Product>> LoadCategory(List<Product> products);
    public Task<Product?> GetByCodeAsync(string code, int? excludedId = null);
    public Task SyncCategoriesAsync(Product product, IEnumerable<int> categoryIds);
}

