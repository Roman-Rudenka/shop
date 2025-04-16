using MediatR;
using Shop.Core.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Application.Commands.Products
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public Guid ProductId { get; }

        public DeleteProductCommand(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
                throw new KeyNotFoundException("Продукт не найден.");

            await _productRepository.DeleteProductAsync(product);
            return Unit.Value;
        }
    }
}
