using Microsoft.AspNetCore.Mvc;
using Sales.API.Interfaces.Services;
using Sales.API.Models;
using Sales.API.Models.Entities;
using Sales.API.Models.Responses;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Retrieves a paginated list of carts.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="size">Page size (default: 10)</param>
        /// <param name="order">Sorting order (default: "id desc")</param>
        /// <returns>Paginated list of carts</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all carts", Description = "Retrieves a paginated list of carts.")]
        [SwaggerResponse(200, "Successful operation", typeof(PaginatedResult<Cart>))]
        public async Task<ActionResult<PaginatedResult<Cart>>> GetAllCarts(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string order = "id desc")
        {
            var result = await _cartService.GetAllCartsAsync(page, size, order);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a cart by its ID.
        /// </summary>
        /// <param name="id">Cart ID</param>
        /// <returns>Cart details</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get cart by ID", Description = "Retrieves a cart by its ID.")]
        [SwaggerResponse(200, "Successful operation", typeof(Cart))]
        [SwaggerResponse(404, "Cart not found", typeof(ApiErrorResponse))]
        public async Task<ActionResult<Cart>> GetCartById(int id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound(new ApiErrorResponse("ResourceNotFound", "Cart not found", $"The Cart with ID {id} does not exist in our database"));
            }
            return Ok(cart);
        }

        /// <summary>
        /// Creates a new cart.
        /// </summary>
        /// <param name="cart">Cart data</param>
        /// <returns>Created cart</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new cart", Description = "Creates a new cart.")]
        [SwaggerResponse(201, "Cart created successfully", typeof(Cart))]
        [SwaggerResponse(400, "Invalid cart data", typeof(ApiErrorResponse))]
        public async Task<ActionResult<Cart>> CreateCart([FromBody] Cart cart)
        {
            if (cart == null)
            {
                return BadRequest(new ApiErrorResponse("ValidationError", "Invalid cart data", "Inform a valid cart data"));
            }

            var createdCart = await _cartService.CreateCartAsync(cart);

            return CreatedAtAction(nameof(GetCartById), new { id = createdCart.Id }, createdCart);
        }

        /// <summary>
        /// Updates an existing cart.
        /// </summary>
        /// <param name="id">Cart ID</param>
        /// <param name="cart">Updated cart data</param>
        /// <returns>Updated cart</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update cart", Description = "Updates an existing cart.")]
        [SwaggerResponse(200, "Cart updated successfully", typeof(Cart))]
        [SwaggerResponse(400, "Invalid cart data", typeof(ApiErrorResponse))]
        [SwaggerResponse(404, "Cart not found", typeof(ApiErrorResponse))]
        public async Task<ActionResult<Cart>> UpdateCart(int id, [FromBody] Cart cart)
        {
            if (cart == null)
            {
                return BadRequest(new ApiErrorResponse("ValidationError", "Invalid cart data", "Inform a valid cart data"));
            }

            var updatedCart = await _cartService.UpdateCartAsync(id, cart);

            if (updatedCart == null)
            {
                return NotFound(new ApiErrorResponse("ResourceNotFound", "Cart not found", $"The Cart with ID {id} does not exist in our database"));
            }

            return Ok(updatedCart);
        }

        /// <summary>
        /// Deletes a cart by its ID.
        /// </summary>
        /// <param name="id">Cart ID</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete cart", Description = "Deletes a cart by its ID.")]
        [SwaggerResponse(204, "Cart deleted successfully")]
        [SwaggerResponse(404, "Cart not found", typeof(ApiErrorResponse))]
        public async Task<ActionResult> DeleteCart(int id)
        {
            var success = await _cartService.DeleteCartAsync(id);
            if (!success)
            {
                return NotFound(new ApiErrorResponse("ResourceNotFound", "Cart not found", $"The Cart with ID {id} does not exist in our database"));
            }

            return NoContent();
        }
    }
}
