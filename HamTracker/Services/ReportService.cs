using System;
using System.Collections.Generic;
using System.IO;
using HamTracker.Models;

namespace HamTracker.Services
{
    public class ReportService
    {
        public void GenerateTextReport(Project project,
                                       List<TaskItem> tasks,
                                       List<Evidence> evidences,
                                       string outputPath)
        {
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("================================================");
                writer.WriteLine("         HAMTRACKER — PROJECT REPORT");
                writer.WriteLine("================================================");
                writer.WriteLine("Project  : " + project.Name);
                writer.WriteLine("Client   : " + project.ClientName);
                writer.WriteLine("Status   : " + project.Status);
                writer.WriteLine("Start    : " + project.StartDate.ToString("dd MMM yyyy"));
                writer.WriteLine("Generated: " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));
                writer.WriteLine("------------------------------------------------");
                writer.WriteLine();
                writer.WriteLine("TASKS & EVIDENCE");
                writer.WriteLine("------------------------------------------------");

                foreach (var task in tasks)
                {
                    writer.WriteLine("Task     : " + task.Title);
                    writer.WriteLine("Status   : " + task.Status);
                    writer.WriteLine("Created  : " + task.CreatedAt.ToString("dd MMM yyyy HH:mm"));

                    var ev = evidences.Find(e => e.TaskId == task.TaskId);
                    if (ev != null)
                    {
                        writer.WriteLine("Evidence : " + ev.FileName);
                        writer.WriteLine("Uploaded : " + ev.UploadedAt.ToString("dd MMM yyyy HH:mm"));
                        writer.WriteLine("Verified : " + (ev.IsVerified ? "YES" : "PENDING"));
                        writer.WriteLine("SHA-256  : " + ev.FileHash);
                    }
                    else
                    {
                        writer.WriteLine("Evidence : None uploaded");
                    }
                    writer.WriteLine("------------------------------------------------");
                }
            }
        }
    }
}