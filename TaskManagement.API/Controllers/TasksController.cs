using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs;
using TaskManagement.API.DTOs.TaskManagement.API.DTOs;
using TaskManagement.API.Models;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Only Manager can create task
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult CreateTask(CreateTaskDto dto)
        {
            var managerEmail = User.FindFirstValue(ClaimTypes.Name);
            var manager = _context.Users.FirstOrDefault(x => x.Email == managerEmail);

            if (manager == null)
                return Unauthorized();

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Deadline = dto.Deadline,
                AssignedToUserId = dto.AssignedToUserId,
                CreatedByManagerId = manager.Id,
                Status = "ToDo"
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return Ok(task);
        }

        // ✅ User can see only their tasks
        [Authorize(Roles = "User")]
        [HttpGet("my-tasks")]
        public IActionResult GetMyTasks()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return Unauthorized();

            var tasks = _context.Tasks
                .Where(t => t.AssignedToUserId == user.Id)
                .ToList();

            return Ok(tasks);
        }

        // ✅ Admin can see all tasks
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("all")]
        public IActionResult GetAllTasks()
        {
            return Ok(_context.Tasks.ToList());
        }


        // ✅ Update Task (User, Manager, Admin)
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, UpdateTaskDto dto)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound("Task not found");

            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return Unauthorized();

            // Role-based security
            if (user.Role == "User" && task.AssignedToUserId != user.Id)
                return Forbid("You can update only your tasks");

            if (user.Role == "Manager" && task.CreatedByManagerId != user.Id)
                return Forbid("You can update only tasks you created");

            // Update fields
            if (!string.IsNullOrEmpty(dto.Status))
                task.Status = dto.Status;

            if (!string.IsNullOrEmpty(dto.Priority))
                task.Priority = dto.Priority;

            if (dto.Deadline.HasValue)
                task.Deadline = dto.Deadline.Value;

            _context.SaveChanges();

            return Ok(task);
        }
        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
