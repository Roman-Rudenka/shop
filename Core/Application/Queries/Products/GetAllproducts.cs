using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;


namespace Shop.Core.Application.Queries.Products
{
    public class GetAllProductsQuery : IRequest<IEnumerable<Product>> { }

    public class GetAllVisibleProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllVisibleProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetAllVisibleProductsAsync();
        }
    }
}
