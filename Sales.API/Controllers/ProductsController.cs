using Microsoft.AspNetCore.Mvc;
using Sales.API.Models;
using Sales.API.Models.Entities;
using Sales.API.Models.Responses;
using Sales.API.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Sales.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retrieve all products with pagination
    /// </summary>
    /// <param name="_page">Page number for pagination</param>
    /// <param name="_size">Number of products per page</param>
    /// <param name="_order">Sorting order</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Retrieve all products", Description = "Fetches a paginated list of products.")]
    [SwaggerResponse(200, "Successfully retrieved products", typeof(IEnumerable<Product>))]
    public async Task<IActionResult> GetProducts([FromQuery] int _page = 1, [FromQuery] int _size = 10, [FromQuery] string _order = null)
    {
        var result = await _productService.GetProductsAsync(_page, _size, _order);
        return Ok(result);
    }

    /// <summary>
    /// Retrieve product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product object</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve product by ID", Description = "Fetches a specific product by ID.")]
    [SwaggerResponse(200, "Product found", typeof(Product))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new ApiErrorResponse("ResourceNotFound", "Product not found", $"The product with ID {id} does not exist in our database"));
        }
        return Ok(product);
    }

    /// <summary>
    /// Add a new product
    /// </summary>
    /// <param name="product">Product object</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new product", Description = "Creates a new product in the system.")]
    [SwaggerResponse(201, "Product created successfully", typeof(Product))]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        var createdProduct = await _productService.AddProductAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product object</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing product", Description = "Updates the details of an existing product.")]
    [SwaggerResponse(200, "Product updated successfully", typeof(Product))]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        var updatedProduct = await _productService.UpdateProductAsync(id, product);
        if (updatedProduct == null)
        {
            return NotFound(new ApiErrorResponse("ResourceNotFound", "Product not found", $"The product with ID {id} does not exist in our database"));
        }
        return Ok(updatedProduct);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a product", Description = "Removes a product from the system by ID.")]
    [SwaggerResponse(204, "Product deleted successfully")]
    [SwaggerResponse(404, "Product not found")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var isDeleted = await _productService.DeleteProductAsync(id);
        if (!isDeleted)
        {
            return NotFound(new ApiErrorResponse("ResourceNotFound", "Product not found", $"The product with ID {id} does not exist in our database"));
        }
        return NoContent();
    }
}
