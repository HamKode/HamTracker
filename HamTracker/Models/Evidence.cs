using System;

namespace HamTracker.Models
{
    public class Evidence
    {
        public int EvidenceId { get; set; }

        public int TaskId { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string FileHash { get; set; } // SHA-256

        public string Description { get; set; }

        public DateTime UploadedAt { get; set; }

        public bool IsVerified { get; set; }
    }
}