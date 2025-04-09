using Shop.Core.Entities;
using Shop.Core.Interfaces;

namespace Shop.Core.UseCases.Commands
{
    public class ProductCommands
    {
        private readonly IProductRepository _productRepository;

        public ProductCommands(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProductAsync(string name, string description, decimal price, Guid publisherId)
        {
            if (publisherId == Guid.Empty)
            {
                throw new ArgumentException("PublisherId cannot be empty.", nameof(publisherId));
            }

            var product = new Product(name, description, price, publisherId);

            await _productRepository.AddProductAsync(product);
        }


        public async Task UpdateProductAsync(Guid productId, string name, string description, decimal price)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Продукт не найден.");

            product.Update(name, description, price);
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Продукт не найден.");

            await _productRepository.DeleteProductAsync(product);
        }
        public async Task HideProductsByPublisherAsync(Guid publisherId)
        {
            await _productRepository.HideProductsByPublisherAsync(publisherId);
        }

        public async Task ShowProductsByPublisherAsync(Guid publisherId)
        {
            await _productRepository.ShowProductsByPublisherAsync(publisherId);
        }
    }
}
