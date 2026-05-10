using System;

namespace HamTracker.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ClientName { get; set; }

        public DateTime StartDate { get; set; }

        public string Status { get; set; } // Active, Completed, Paused
    }
}