using Shop.Core.Domain.Entities;

namespace Shop.Core.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllVisibleProductsAsync();
        Task<IEnumerable<Product>> GetProductsByPublisherAsync(Guid publisherId);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task HideProductsByPublisherAsync(Guid publisherId);
        Task ShowProductsByPublisherAsync(Guid publisherId);
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> FilterByPriceAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> FilterBySellerAsync(Guid sellerId);
    }
}
