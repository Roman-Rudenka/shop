using Shop.Core.Entities;
using Shop.Core.Interfaces;

namespace Shop.Core.UseCases.Queries
{
    public class ProductQueries
    {
        private readonly IProductRepository _productRepository;

        public ProductQueries(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllVisibleProductsAsync()
        {
            return await _productRepository.GetAllVisibleProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPublisherAsync(Guid publisherId)
        {
            return await _productRepository.GetProductsByPublisherAsync(publisherId);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            return await _productRepository.SearchByNameAsync(name);
        }

        public async Task<IEnumerable<Product>> FilterByPriceAsync(decimal minPrice, decimal maxPrice)
        {
            return await _productRepository.FilterByPriceAsync(minPrice, maxPrice);
        }

        public async Task<IEnumerable<Product>> FilterBySellerAsync(Guid sellerId)
        {
            return await _productRepository.FilterBySellerAsync(sellerId);
        }
    }
}
