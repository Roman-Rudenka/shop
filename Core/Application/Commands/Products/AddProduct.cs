using MediatR;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;
using System;

namespace Shop.Core.Application.Commands.Products
{
    public class AddProductCommand : IRequest<Unit>
    {
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public Guid PublisherId { get; }

        public AddProductCommand(string name, string description, decimal price, Guid publisherId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Price = price;
            PublisherId = publisherId;
        }
    }

    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public AddProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product(request.Name, request.Description, request.Price, request.PublisherId);
            await _productRepository.AddProductAsync(product);

            return Unit.Value;
        }
    }

}
