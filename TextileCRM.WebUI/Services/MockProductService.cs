using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextileCRM.Application.Interfaces;
using TextileCRM.Domain.Entities;

namespace TextileCRM.WebUI.Services
{
    public class MockProductService : IProductService
    {
        private readonly List<Product> _products;

        public MockProductService()
        {
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Pamuklu Kumaş", Description = "Yüksek kalite pamuklu kumaş", Category = ProductCategory.Fabric, Price = 15.99m, StockQuantity = 100 },
                new Product { Id = 2, Name = "Polyester Kumaş", Description = "Dayanıklı polyester kumaş", Category = ProductCategory.Fabric, Price = 12.50m, StockQuantity = 150 },
                new Product { Id = 3, Name = "Düğme Seti", Description = "Çeşitli renklerde düğme seti", Category = ProductCategory.Accessory, Price = 5.99m, StockQuantity = 200 },
                new Product { Id = 4, Name = "Fermuar", Description = "Metal fermuar", Category = ProductCategory.Accessory, Price = 2.99m, StockQuantity = 300 },
                new Product { Id = 5, Name = "Dikiş İpliği", Description = "Yüksek kalite dikiş ipliği", Category = ProductCategory.Thread, Price = 3.50m, StockQuantity = 250 }
            };
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await Task.FromResult(_products);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category)
        {
            return await Task.FromResult(_products.Where(p => p.Category == category).ToList());
        }

        public async Task CreateProductAsync(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
            await Task.CompletedTask;
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            await Task.CompletedTask;
        }

        public async Task UpdateStockAsync(int id, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.StockQuantity = quantity;
            }
            await Task.CompletedTask;
        }
    }
}