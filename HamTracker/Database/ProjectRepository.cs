using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HamTracker.Models;

namespace HamTracker.Database
{
    public class ProjectRepository
    {
        public void Insert(Project p)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Projects (Name, Description, ClientName, StartDate, Status)
                        VALUES (@name, @desc, @client, @start, @status)";
                    cmd.Parameters.AddWithValue("@name", p.Name);
                    cmd.Parameters.AddWithValue("@desc", p.Description);
                    cmd.Parameters.AddWithValue("@client", p.ClientName);
                    cmd.Parameters.AddWithValue("@start", p.StartDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@status", p.Status);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Project> GetAll()
        {
            var list = new List<Project>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Projects ORDER BY ProjectId DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Project
                            {
                                ProjectId = Convert.ToInt32(reader["ProjectId"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                ClientName = reader["ClientName"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void Delete(int projectId)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Projects WHERE ProjectId = @id";
                    cmd.Parameters.AddWithValue("@id", projectId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateStatus(int projectId, string status)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Projects SET Status = @status WHERE ProjectId = @id";
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@id", projectId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
