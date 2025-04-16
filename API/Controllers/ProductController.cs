using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Shop.Core.Application.Commands.Products;
using Shop.Core.Application.Queries.Products;
using Shop.Core.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.API.Controllers
{
    /// <summary>
    /// Контроллер для управления продуктами.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Конструктор контроллера продуктов.
        /// </summary>
        /// <param name="mediator">Интерфейс для взаимодействия с запросами и командами.</param>
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Получить список всех доступных продуктов.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllVisibleProducts()
        {
            var query = new GetAllProductsQuery();
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        /// <summary>
        /// Получить продукты по идентификатору продавца.
        /// </summary>
        /// <param name="sellerId">Идентификатор продавца.</param>
        [HttpGet("seller/{sellerId}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> GetProductsBySeller(Guid sellerId)
        {
            var query = new GetProductsByPublisherQuery(sellerId);
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        /// <summary>
        /// Получить продукт по идентификатору.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var query = new GetProductByIdQuery(productId);
            var product = await _mediator.Send(query);

            if (product == null)
                return NotFound(new { Message = "Продукт не найден." });

            return Ok(product);
        }

        /// <summary>
        /// Добавить новый продукт.
        /// </summary>
        /// <param name="product">Данные продукта.</param>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
                return Unauthorized(new { Message = "UserId claim не найден." });

            if (!Guid.TryParse(userIdClaim.Value, out var publisherId))
                return BadRequest(new { Message = "Некорректный формат UserId." });

            var command = new AddProductCommand(product.Name!, product.Description!, product.Price, publisherId);
            await _mediator.Send(command);

            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }

        /// <summary>
        /// Обновить существующий продукт.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        /// <param name="updatedProduct">Обновленные данные продукта.</param>
        [HttpPut("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateProductCommand(productId, updatedProduct.Name!, updatedProduct.Description!, updatedProduct.Price);
            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Удалить продукт.
        /// </summary>
        /// <param name="productId">Идентификатор продукта.</param>
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var command = new DeleteProductCommand(productId);
            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Скрыть продукты продавца.
        /// </summary>
        /// <param name="sellerId">Идентификатор продавца.</param>
        [HttpPost("hide/{sellerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideProductsBySeller(Guid sellerId)
        {
            var command = new HideProductsByPublisherCommand(sellerId);
            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Показать продукты продавца.
        /// </summary>
        /// <param name="sellerId">Идентификатор продавца.</param>
        [HttpPost("show/{sellerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowProductsBySeller(Guid sellerId)
        {
            var command = new ShowProductsByPublisherCommand(sellerId);
            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Найти продукты по названию.
        /// </summary>
        /// <param name="name">Название продукта.</param>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            var query = new SearchProductsByNameQuery(name);
            var products = await _mediator.Send(query);

            return Ok(products);
        }

        /// <summary>
        /// Фильтр продуктов по цене.
        /// </summary>
        /// <param name="minPrice">Минимальная цена.</param>
        /// <param name="maxPrice">Максимальная цена.</param>
        [HttpGet("filter/price")]
        public async Task<IActionResult> FilterByPrice([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var query = new FilterByPriceQuery(minPrice, maxPrice);
            var products = await _mediator.Send(query);

            return Ok(products);
        }

        /// <summary>
        /// Фильтр продуктов по продавцу.
        /// </summary>
        /// <param name="sellerId">Идентификатор продавца.</param>
        [HttpGet("filter/seller")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> FilterBySeller([FromQuery] Guid sellerId)
        {
            var query = new FilterBySellerQuery(sellerId);
            var products = await _mediator.Send(query);

            return Ok(products);
        }
    }
}
