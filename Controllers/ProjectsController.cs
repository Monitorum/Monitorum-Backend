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


            var projects = _dbContext.Projects.Where(p => p.OwnerId == int.Parse(userId!)).ToListAsync();

            return Ok(new
            {
                data = projects.Result,
            });


            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == int.Parse(userId!));

            if (user == null)
                return Unauthorized(new { message = "User not found" });


            return Ok(user.Projects);
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
                Owner = user,
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
    }

    public class ProjectCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
