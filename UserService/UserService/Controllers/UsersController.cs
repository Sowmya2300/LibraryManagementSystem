using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly UserDbContext _context;

		public UsersController(UserDbContext context)
		{
			_context = context;
		}

		// GET: api/users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _context.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUser),
                new { id = user.Id },
                new
                {
                    status = "success",
                    message = "User added successfully.",
                    data = user
                }
            );
        }


        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "User ID in the URL does not match the ID in the request body."
                });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(u => u.Id == id))
                {
                    return NotFound(new
                    {
                        status = "error",
                        message = $"User with ID {id} not found."
                    });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new
            {
                status = "success",
                message = "User updated successfully.",
                data = user
            });
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = "User deleted successfully."
            });
        }
	}
}
