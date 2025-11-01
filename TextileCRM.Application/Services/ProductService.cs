using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category)
        {
            return await _productRepository.FindAsync(p => p.Category == category);
        }

        public async Task CreateProductAsync(Product product)
        {
            product.CreatedDate = DateTime.Now;
            var allProducts = await _productRepository.GetAllAsync();
            var maxId = allProducts.Any() ? allProducts.Max(p => p.Id) : 0;
            product.Code = $"URN-{(maxId + 1):D4}";
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.StockQuantity = quantity;
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();
            }
        }
    }
}