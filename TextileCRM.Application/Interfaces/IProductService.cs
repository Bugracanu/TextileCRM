using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task UpdateStockAsync(int id, int quantity);
    }
}