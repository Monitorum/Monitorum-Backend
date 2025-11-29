namespace Monitorum.Models
{
    public enum RoleEnum
    {
        Manager,
        Executor
    }
    public class Member
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public RoleEnum Role { get; set; }
        public Team Team { get; set; } = null!;
        public int TeamId { get; set; }
    }
}