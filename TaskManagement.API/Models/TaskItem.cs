namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Status { get; set; } = "ToDo";
        public string Priority { get; set; }

        public DateTime Deadline { get; set; }

        // Assigned user
        public int AssignedToUserId { get; set; }
        public User AssignedToUser { get; set; }

        // 👇 ADD THIS (Manager who created task)
        public int CreatedByManagerId { get; set; }
    }
}
