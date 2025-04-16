using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;

namespace Shop.Core.Application.Queries.Products
{
    public class GetProductsByPublisherQuery : IRequest<IEnumerable<Product>>
    {
        public Guid PublisherId { get; }

        public GetProductsByPublisherQuery(Guid publisherId)
        {
            PublisherId = publisherId;
        }
    }

    public class GetProductsByPublisherHandler : IRequestHandler<GetProductsByPublisherQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByPublisherHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductsByPublisherQuery request, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProductsByPublisherAsync(request.PublisherId);
        }
    }
}
