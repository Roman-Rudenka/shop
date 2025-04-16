using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Queries.Products
{
    public class SearchProductsByNameQuery : IRequest<IEnumerable<Product>>
    {
        public string Name { get; }

        public SearchProductsByNameQuery(string name)
        {
            Name = name;
        }
    }

    public class SearchProductsByNameHandler : IRequestHandler<SearchProductsByNameQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public SearchProductsByNameHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(SearchProductsByNameQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.SearchByNameAsync(request.Name);
        }
    }
}
