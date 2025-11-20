using Microsoft.EntityFrameworkCore;
using Monitorum.Models;

namespace Monitorum.Data
{
    public class MonitorumDbContext: DbContext
    {
        public MonitorumDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

    }
}
