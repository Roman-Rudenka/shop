using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.Entities;
using Shop.Core.UseCases.Commands;
using Shop.Core.UseCases.Queries;

namespace Shop.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductCommands _productCommands;
        private readonly ProductQueries _productQueries;

        public ProductsController(ProductCommands productCommands, ProductQueries productQueries)
        {
            _productCommands = productCommands;
            _productQueries = productQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisibleProducts()
        {
            var products = await _productQueries.GetAllVisibleProductsAsync();
            return Ok(products);
        }

        [HttpGet("seller/{sellerId}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> GetProductsBySeller(Guid sellerId)
        {
            var products = await _productQueries.GetProductsByPublisherAsync(sellerId);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var product = await _productQueries.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound("Продукт не найден.");
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "UserId claim not found in token." });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var publisherId))
            {
                return BadRequest(new { Message = "Invalid UserId format." });
            }

            await _productCommands.AddProductAsync(product.Name!, product.Description!, product.Price, publisherId);

            return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product);
        }



        [HttpPut("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productCommands.UpdateProductAsync(productId, updatedProduct.Name!, updatedProduct.Description!, updatedProduct.Price);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            await _productCommands.DeleteProductAsync(productId);
            return NoContent();
        }

        [HttpPost("hide/{sellerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideProductsBySeller(Guid sellerId)
        {
            await _productCommands.HideProductsByPublisherAsync(sellerId);
            return NoContent();
        }

        [HttpPost("show/{sellerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowProductsBySeller(Guid sellerId)
        {
            await _productCommands.ShowProductsByPublisherAsync(sellerId);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            var products = await _productQueries.SearchByNameAsync(name);
            return Ok(products);
        }

        [HttpGet("filter/price")]
        public async Task<IActionResult> FilterByPrice([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var products = await _productQueries.FilterByPriceAsync(minPrice, maxPrice);
            return Ok(products);
        }

        [HttpGet("filter/seller")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> FilterBySeller([FromQuery] Guid sellerId)
        {
            var products = await _productQueries.FilterBySellerAsync(sellerId);
            return Ok(products);
        }
    }
}
