using System;

namespace HamTracker.Models
{
    public class AuditLog
    {
        public int LogId { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}