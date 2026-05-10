using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HamTracker.Models;

namespace HamTracker.Database
{
    public class TaskRepository
    {
        public void Insert(TaskItem t)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Tasks (ProjectId, Title, Description, Status, CreatedAt)
                        VALUES (@pid, @title, @desc, @status, @created)";
                    cmd.Parameters.AddWithValue("@pid", t.ProjectId);
                    cmd.Parameters.AddWithValue("@title", t.Title);
                    cmd.Parameters.AddWithValue("@desc", t.Description);
                    cmd.Parameters.AddWithValue("@status", t.Status);
                    cmd.Parameters.AddWithValue("@created", t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<TaskItem> GetByProject(int projectId)
        {
            var list = new List<TaskItem>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Tasks WHERE ProjectId = @pid ORDER BY TaskId DESC";
                    cmd.Parameters.AddWithValue("@pid", projectId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TaskItem
                            {
                                TaskId = Convert.ToInt32(reader["TaskId"]),
                                ProjectId = Convert.ToInt32(reader["ProjectId"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                CompletedAt = reader["CompletedAt"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["CompletedAt"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<TaskItem> GetAll()
        {
            var list = new List<TaskItem>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Tasks ORDER BY TaskId DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TaskItem
                            {
                                TaskId = Convert.ToInt32(reader["TaskId"]),
                                ProjectId = Convert.ToInt32(reader["ProjectId"]),
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Status = reader["Status"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                CompletedAt = reader["CompletedAt"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["CompletedAt"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void UpdateStatus(int taskId, string status)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    string completedAt = status == "Done"
                        ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        : null;
                    cmd.CommandText = @"
                        UPDATE Tasks SET Status = @status, CompletedAt = @completed
                        WHERE TaskId = @id";
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@completed", (object)completedAt ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int taskId)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Tasks WHERE TaskId = @id";
                    cmd.Parameters.AddWithValue("@id", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}