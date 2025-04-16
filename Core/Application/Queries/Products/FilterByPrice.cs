using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Queries.Products
{
    public class FilterByPriceQuery : IRequest<IEnumerable<Product>>
    {
        public decimal MinPrice { get; }
        public decimal MaxPrice { get; }

        public FilterByPriceQuery(decimal minPrice, decimal maxPrice)
        {
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }
    }

    public class FilterByPriceHandler : IRequestHandler<FilterByPriceQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public FilterByPriceHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(FilterByPriceQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.FilterByPriceAsync(request.MinPrice, request.MaxPrice);
        }
    }
}
