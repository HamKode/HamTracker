using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HamTracker.Models;

namespace HamTracker.Database
{
    public class EvidenceRepository
    {
        public void Insert(Evidence e)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Evidence (TaskId, FilePath, FileName, FileHash, Description, UploadedAt, IsVerified)
                        VALUES (@tid, @path, @fname, @hash, @desc, @uploaded, @verified)";
                    cmd.Parameters.AddWithValue("@tid", e.TaskId);
                    cmd.Parameters.AddWithValue("@path", e.FilePath);
                    cmd.Parameters.AddWithValue("@fname", e.FileName);
                    cmd.Parameters.AddWithValue("@hash", e.FileHash);
                    cmd.Parameters.AddWithValue("@desc", e.Description);
                    cmd.Parameters.AddWithValue("@uploaded", e.UploadedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@verified", e.IsVerified ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Evidence> GetByTask(int taskId)
        {
            var list = new List<Evidence>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Evidence WHERE TaskId = @tid ORDER BY EvidenceId DESC";
                    cmd.Parameters.AddWithValue("@tid", taskId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapEvidence(reader));
                        }
                    }
                }
            }
            return list;
        }

        public List<Evidence> GetAll()
        {
            var list = new List<Evidence>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Evidence ORDER BY EvidenceId DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapEvidence(reader));
                        }
                    }
                }
            }
            return list;
        }

        public void SetVerified(int evidenceId, bool verified)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Evidence SET IsVerified = @v WHERE EvidenceId = @id";
                    cmd.Parameters.AddWithValue("@v", verified ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", evidenceId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Evidence MapEvidence(SQLiteDataReader reader)
        {
            return new Evidence
            {
                EvidenceId = Convert.ToInt32(reader["EvidenceId"]),
                TaskId = Convert.ToInt32(reader["TaskId"]),
                FilePath = reader["FilePath"].ToString(),
                FileName = reader["FileName"].ToString(),
                FileHash = reader["FileHash"].ToString(),
                Description = reader["Description"].ToString(),
                UploadedAt = Convert.ToDateTime(reader["UploadedAt"]),
                IsVerified = Convert.ToInt32(reader["IsVerified"]) == 1
            };
        }
    }
}