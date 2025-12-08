namespace Monitorum.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public ICollection<Member> Members { get; set; } = new List<Member>();

        public bool IsTeamManager(int teamId)
        {
            return (from member in Members
             where member.TeamId == teamId
             select member.Role == RoleEnum.Manager).FirstOrDefault();
        }
    }
}
