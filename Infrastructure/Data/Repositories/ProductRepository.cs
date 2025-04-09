using Microsoft.EntityFrameworkCore;
using Shop.Core.Entities;
using Shop.Core.Interfaces;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllVisibleProductsAsync()
        {
            return await _context.Products.Where(p => p.IsAvailable).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPublisherAsync(Guid publisherId)
        {
            return await _context.Products.Where(p => p.PublisherId == publisherId).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task HideProductsByPublisherAsync(Guid publisherId)
        {
            var products = await _context.Products.Where(p => p.PublisherId == publisherId).ToListAsync();
            foreach (var product in products)
            {
                product.NotAvailable();
            }
            await _context.SaveChangesAsync();
        }

        public async Task ShowProductsByPublisherAsync(Guid publisherId)
        {
            var products = await _context.Products.Where(p => p.PublisherId == publisherId).ToListAsync();
            foreach (var product in products)
            {
                product.Available();
            }
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _context.Products
                .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterByPriceAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterBySellerAsync(Guid sellerId)
        {
            return await _context.Products
                .Where(p => p.PublisherId == sellerId)
                .ToListAsync();
        }
    }
}
