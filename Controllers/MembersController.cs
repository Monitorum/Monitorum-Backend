using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitorum.Data;
using Monitorum.Models;

namespace Monitorum.Controllers
{
    [Route("api/teams/{teamId}/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly MonitorumDbContext _context;

        public MembersController(MonitorumDbContext dbContext)
        {
            _context = dbContext;
        }
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMembers(int teamId)
        {
            /////////not working
            var Team = await _context.Teams.FindAsync(teamId);
            if (Team == null)
            {
                return BadRequest();
            }
            return Ok(Team.Members.Select(m=>m.Id));
        }
        //[Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> CreateMember(int teamId, int id)
        {
            var Team = await _context.Teams.FindAsync(teamId);
            if (Team == null)
            {
                return BadRequest();
            }

            
            Member member =new Member{
                TeamId = teamId,
                UserId = id,
                Role = RoleEnum.Executor
            };

            //await _context.Members.AddAsync(member);
            Team.Members.Add(member);

            await _context.SaveChangesAsync();
            return Ok();
        }
        //[Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int teamId, int id)
        {
            var Team = await _context.Teams.FindAsync(teamId);
            if (Team == null)
            {
                return BadRequest("not Team");
            }

            var Member = await _context.Members.FindAsync(id);
            if (Member == null)
            {
                return BadRequest("not Member");
            }

            _context.Members.Remove(Member!);
            await _context.SaveChangesAsync();
            return Ok();
        }
    };
    public class MemberDto
    {
        public string Membername { get; set; } = string.Empty;
    }
}