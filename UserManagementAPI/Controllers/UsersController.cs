using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Interfaces;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository repository, ILogger<UsersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_repository.GetAllUsers());
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            try
            {
                var user = _repository.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { Error = "User not found", UserId = id });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID {UserId}", id);
                return StatusCode(500, new { Error = "An internal server error occurred." });
            }
        }

        // POST: api/users
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddUser([FromBody] User user)
        {
            _repository.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();

            var success = _repository.UpdateUser(user);
            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var success = _repository.DeleteUser(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
