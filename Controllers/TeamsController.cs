using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitorum.Data;
using Monitorum.Models;

namespace Monitorum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase 
    {
        private readonly MonitorumDbContext _context;

        public TeamsController(MonitorumDbContext dbContext)
        {
            _context = dbContext;
        }
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var Teams = await _context.Teams.ToListAsync();
            return Ok(Teams);
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTeam(TeamDto request)
        {
            await _context.Teams.AddAsync(new Team 
            {
                Name = request.Teamname   
            }
            );
            await _context.SaveChangesAsync();
            return Ok();
        }
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var Team = await _context.Teams.FindAsync(id);
            if (Team == null)
            {
                return BadRequest();
            }
            _context.Teams.Remove(Team!);
            await _context.SaveChangesAsync();
            return Ok();
        }
    };
    public class TeamDto
    {
        public string Teamname { get; set; } = string.Empty;
    }
}