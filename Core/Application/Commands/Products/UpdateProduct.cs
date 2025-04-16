using MediatR;
using Shop.Core.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Application.Commands.Products
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }

        public UpdateProductCommand(Guid productId, string name, string description, decimal price)
        {
            ProductId = productId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Price = price;
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Продукт не найден.");

            product.Update(request.Name, request.Description, request.Price);
            await _productRepository.UpdateProductAsync(product);

            return Unit.Value;
        }
    }
}
