using Microsoft.EntityFrameworkCore;
using Monitorum.Models;

namespace Monitorum.Data
{
    public class MonitorumDbContext: DbContext
    {
        public MonitorumDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Member> Members { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectTask> ProjectTasks { get; set; } = null!;
    }
}
