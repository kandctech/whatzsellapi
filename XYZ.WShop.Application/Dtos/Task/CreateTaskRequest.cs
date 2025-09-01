

namespace XYZ.WShop.Application.Dtos.Task
{
    public class CreateTaskRequest
    {
        public Guid BusinessId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string Priority { get; set; } = "medium";
        public string Category { get; set; } = "work";
        public bool Completed { get; set; } = false;
    }
}
