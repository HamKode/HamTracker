using System;
using System.IO;
using HamTracker.Models;

namespace HamTracker.Services
{
    public class EvidenceService
    {
        private readonly string _evidenceFolder = "EvidenceFiles";

        public EvidenceService()
        {
            Directory.CreateDirectory(_evidenceFolder);
        }

        public Evidence PrepareEvidence(string sourceFilePath, int taskId, string description)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string destPath = Path.Combine(_evidenceFolder, timestamp + "_" + fileName);

            File.Copy(sourceFilePath, destPath, true);

            string hash = HashService.ComputeFileHash(destPath);

            return new Evidence
            {
                TaskId = taskId,
                FilePath = destPath,
                FileName = fileName,
                FileHash = hash,
                Description = description,
                UploadedAt = DateTime.Now,
                IsVerified = false
            };
        }
    }
}