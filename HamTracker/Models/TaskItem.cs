using System;

namespace HamTracker.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }

        public int ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; } // ToDo, InProgress, Done

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}
