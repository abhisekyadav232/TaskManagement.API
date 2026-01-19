using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/users/me
        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            if (email == null)
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

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
    }
}
