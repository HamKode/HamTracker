using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HamTracker.Models;

namespace HamTracker.Database
{
    public class AuditLogRepository
    {
        public void Log(string action, string entityType, int entityId, string details)
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO AuditLog (Action, EntityType, EntityId, Details, Timestamp)
                        VALUES (@action, @entity, @eid, @details, @ts)";
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@entity", entityType);
                    cmd.Parameters.AddWithValue("@eid", entityId);
                    cmd.Parameters.AddWithValue("@details", details);
                    cmd.Parameters.AddWithValue("@ts", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<AuditLog> GetAll()
        {
            var list = new List<AuditLog>();
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM AuditLog ORDER BY LogId DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AuditLog
                            {
                                LogId = Convert.ToInt32(reader["LogId"]),
                                Action = reader["Action"].ToString(),
                                EntityType = reader["EntityType"].ToString(),
                                EntityId = Convert.ToInt32(reader["EntityId"]),
                                Details = reader["Details"].ToString(),
                                Timestamp = Convert.ToDateTime(reader["Timestamp"])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}