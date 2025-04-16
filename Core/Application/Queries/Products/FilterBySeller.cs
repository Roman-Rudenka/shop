using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;


namespace Shop.Core.Application.Queries.Products
{
    public class FilterBySellerQuery : IRequest<IEnumerable<Product>>
    {
        public Guid SellerId { get; }

        public FilterBySellerQuery(Guid sellerId)
        {
            SellerId = sellerId;
        }
    }

    public class FilterBySellerHandler : IRequestHandler<FilterBySellerQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public FilterBySellerHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(FilterBySellerQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.FilterBySellerAsync(request.SellerId);
        }
    }
}
