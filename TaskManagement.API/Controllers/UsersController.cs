using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize] // any logged-in user
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/users/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            if (email == null)
                return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return NotFound();

            var result = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(result);
        }

        // ✅ NEW ENDPOINT: GET api/users
        // Only Admin and Manager can see all users
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }
        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 success
        }

    }
}
