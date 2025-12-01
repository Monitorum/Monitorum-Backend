using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitorum.Data;
using Monitorum.Models;
using System.Security.Claims;

namespace Monitorum.Controllers
{
    [Route("api/Projects/{projectId}/[controller]")]
    [ApiController]
    public class ProjectTasksController : ControllerBase
    {
        private readonly MonitorumDbContext _dbContext;
        public ProjectTasksController(MonitorumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTasks(int projectId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var project = await _dbContext.Projects.Include("Tasks").FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return NotFound(new { message = "Project not found" });

            if (project.OwnerId != userId)
                return Unauthorized();


            // TODO: If das ist Team Project, check if user is member

            // TODOÖ If User ist manager return all
            // TODO: Sonst return only assigned to user

            return Ok(new
            {
                data = project.Tasks,
                count = project.Tasks.Count,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] ProjectTaskCreateDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found" });
            if (project.OwnerId != userId)
                return Unauthorized();


            // TODO: Check ob User is Member und Manager vom Projekt

            var newTask = new ProjectTask
            {
                Title = request.Title,
                Description = request.Description,
                ProjectId = projectId,
            };

            _dbContext.ProjectTasks.Add(newTask);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Task created successfully",
                data = newTask,
            });
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTask(int projectId, int id, [FromBody] ProjectTaskUpdateDto request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found" });
            if (project.OwnerId != userId)
                return Unauthorized();

            // TODO: Check ob User is Member und Manager vom Projekt

            var task = await _dbContext.ProjectTasks.FindAsync(id);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            task.Title = request.Title ?? task.Title;
            task.Description = request.Description ?? task.Description;

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Task was succesfully updated",
                task
            });
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int projectId, int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var project = await _dbContext.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound(new { message = "Project not found" });
            if (project.OwnerId != userId)
                return Unauthorized();

            // TODO: Check ob User is Member und Manager vom Projekt

            var task = await _dbContext.ProjectTasks.FindAsync(id);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            _dbContext.ProjectTasks.Remove(task);

            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Task was succesfully deleted",
                task
            });
        }
    }
    public class ProjectTaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    public class ProjectTaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
