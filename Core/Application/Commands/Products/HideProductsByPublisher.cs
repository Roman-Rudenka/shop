using MediatR;
using Shop.Core.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Core.Application.Commands.Products
{
    public class HideProductsByPublisherCommand : IRequest<Unit>
    {
        public Guid PublisherId { get; }

        public HideProductsByPublisherCommand(Guid publisherId)
        {
            PublisherId = publisherId;
        }
    }

    public class HideProductsByPublisherCommandHandler : IRequestHandler<HideProductsByPublisherCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public HideProductsByPublisherCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(HideProductsByPublisherCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.HideProductsByPublisherAsync(request.PublisherId);
            return Unit.Value;
        }
    }
}
