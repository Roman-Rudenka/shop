using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;


namespace Shop.Core.Application.Queries.Products
{
    public class GetProductByIdQuery : IRequest<Product?>
    {
        public Guid ProductId { get; }

        public GetProductByIdQuery(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductByIdAsync(request.ProductId);
        }
    }
}
