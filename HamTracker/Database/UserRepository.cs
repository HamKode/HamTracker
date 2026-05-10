using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using HamTracker.Models;

namespace HamTracker.Database
{
    public class UserRepository
    {
        // ── Password hashing ──────────────────────────────────────
        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        // ── Create users table ────────────────────────────────────
        public void EnsureTable()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            UserId    INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username  TEXT NOT NULL UNIQUE,
                            FullName  TEXT NOT NULL,
                            Email     TEXT,
                            Role      TEXT DEFAULT 'Freelancer',
                            Password  TEXT NOT NULL,
                            CreatedAt TEXT
                        );";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── Register new user ─────────────────────────────────────
        public bool Register(User user)
        {
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT INTO Users (Username, FullName, Email, Role, Password, CreatedAt)
                            VALUES (@u, @fn, @em, @role, @pw, @dt)";
                        cmd.Parameters.AddWithValue("@u", user.Username.Trim().ToLower());
                        cmd.Parameters.AddWithValue("@fn", user.FullName.Trim());
                        cmd.Parameters.AddWithValue("@em", user.Email.Trim());
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        cmd.Parameters.AddWithValue("@pw", HashPassword(user.Password));
                        cmd.Parameters.AddWithValue("@dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false; // username already exists
            }
        }

        // ── Login validation ──────────────────────────────────────
        public User Login(string username, string password)
        {
            string hashed = HashPassword(password);
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT * FROM Users
                        WHERE Username = @u AND Password = @pw
                        LIMIT 1";
                    cmd.Parameters.AddWithValue("@u", username.Trim().ToLower());
                    cmd.Parameters.AddWithValue("@pw", hashed);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ── Check if any user exists ──────────────────────────────
        public bool AnyUserExists()
        {
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Users";
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}