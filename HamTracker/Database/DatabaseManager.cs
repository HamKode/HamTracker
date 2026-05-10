using System.Data.SQLite;

namespace HamTracker.Database
{
    public class DatabaseManager
    {
        private static string _connectionString = "Data Source=hamtracker.db;Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Projects (
                            ProjectId   INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name        TEXT NOT NULL,
                            Description TEXT,
                            ClientName  TEXT,
                            StartDate   TEXT,
                            Status      TEXT DEFAULT 'Active'
                        );

                        CREATE TABLE IF NOT EXISTS Tasks (
                            TaskId      INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProjectId   INTEGER,
                            Title       TEXT NOT NULL,
                            Description TEXT,
                            Status      TEXT DEFAULT 'ToDo',
                            CreatedAt   TEXT,
                            CompletedAt TEXT,
                            FOREIGN KEY(ProjectId) REFERENCES Projects(ProjectId)
                        );

                        CREATE TABLE IF NOT EXISTS Evidence (
                            EvidenceId  INTEGER PRIMARY KEY AUTOINCREMENT,
                            TaskId      INTEGER,
                            FilePath    TEXT,
                            FileName    TEXT,
                            FileHash    TEXT,
                            Description TEXT,
                            UploadedAt  TEXT,
                            IsVerified  INTEGER DEFAULT 0,
                            FOREIGN KEY(TaskId) REFERENCES Tasks(TaskId)
                        );

                        CREATE TABLE IF NOT EXISTS AuditLog (
                            LogId       INTEGER PRIMARY KEY AUTOINCREMENT,
                            Action      TEXT,
                            EntityType  TEXT,
                            EntityId    INTEGER,
                            Details     TEXT,
                            Timestamp   TEXT
                        );
                    ";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}