using Microsoft.AspNetCore.Mvc;
using Sales.API.Interfaces.Services;
using Sales.API.Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <param name="page">Page number for pagination</param>
        /// <param name="size">Number of users per page</param>
        /// <param name="order">Ordering criteria</param>
        /// <returns>List of users</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve all users", Description = "Fetches a paginated list of users.")]
        [SwaggerResponse(200, "Successfully retrieved users", typeof(IEnumerable<User>))]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string order = "username asc")
        {
            var users = await _userService.GetAllUsersAsync(page, size, order);
            return Ok(new
            {
                data = users,
                totalItems = users.Count,
                currentPage = page,
                totalPages = (int)Math.Ceiling((double)users.Count / size)
            });
        }

        /// <summary>
        /// Retrieve user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User object</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve user by ID", Description = "Fetches a specific user by ID.")]
        [SwaggerResponse(200, "User found", typeof(User))]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Adds a new user to the system.")]
        [SwaggerResponse(201, "User created successfully", typeof(User))]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user object</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing user", Description = "Updates the details of an existing user.")]
        [SwaggerResponse(200, "User updated successfully", typeof(User))]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            var updatedUser = await _userService.UpdateUserAsync(id, user);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a user", Description = "Removes a user from the system by ID.")]
        [SwaggerResponse(204, "User deleted successfully")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var isDeleted = await _userService.DeleteUserAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}
