namespace TaskManagement.API.DTOs
{
    public class UpdateTaskDto
    {
        public string Status { get; set; }   // ToDo, InProgress, Done
        public string Priority { get; set; } // Optional update
        public DateTime? Deadline { get; set; }
    }
}
