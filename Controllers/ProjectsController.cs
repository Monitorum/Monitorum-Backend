using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitorum.Data;
using Monitorum.Models;
using System.Security.Claims;

namespace Monitorum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MonitorumDbContext _dbContext;
        public ProjectsController(MonitorumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _dbContext.Users.FindAsync(int.Parse(userId!));
            if (user == null)
                return Unauthorized(new { message = "User not found" });


            var own = _dbContext.Projects.Where(p => p.OwnerId == int.Parse(userId!)).ToListAsync();

            // TODO: Projects where user is member          

            return Ok(new
            {
                data = own.Result,
                count = own.Result.Count,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectCreateDto request)
        {
            var username = User.Identity?.Name;
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
            
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var project = new Project
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = user.Id,
            };

            //user.Projects.Add(project);
            _dbContext.Projects.Add(project);

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Project created successfully",
                project = new
                {
                    project.Id,
                    project.Name,
                    project.Description,
                },
            });
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var project = await _dbContext.Projects.FindAsync(id);

            if (project == null)
                return NotFound(new {message = "Project not found"});

            if (project.OwnerId != userId)
                return Unauthorized();

            // TODO: Check if user is Manager of Project

            project.Name = dto.Name ?? project.Name;
            project.Description = dto.Description ?? project.Description;

            await _dbContext.SaveChangesAsync();

            return Ok(
                new
                {
                    message = "Prjoject updated successfully",
                    project,
                });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var project = await _dbContext.Projects.FindAsync(id);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            if (project.OwnerId != userId)
                return Unauthorized();

            // TODO: Check if user is Manager of Project


            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully" });
        }
    }

    public class ProjectCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class ProjectUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
