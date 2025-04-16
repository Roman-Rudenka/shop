using MediatR;
using Shop.Core.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Application.Commands.Products
{
    public class ShowProductsByPublisherCommand : IRequest<Unit>
    {
        public Guid PublisherId { get; }

        public ShowProductsByPublisherCommand(Guid publisherId)
        {
            PublisherId = publisherId;
        }
    }

    public class ShowProductsByPublisherCommandHandler : IRequestHandler<ShowProductsByPublisherCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public ShowProductsByPublisherCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(ShowProductsByPublisherCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.ShowProductsByPublisherAsync(request.PublisherId);
            return Unit.Value;
        }
    }
}
