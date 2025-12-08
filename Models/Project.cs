namespace Monitorum.Models
{
    public enum ProjectType
    {
        Private,
        Team
    }
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        public ProjectType Type { get; set; } = ProjectType.Private;
        public int OwnerId { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
