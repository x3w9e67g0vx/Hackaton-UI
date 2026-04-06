using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories
{
    public class TaskStatusRepository
    {
        private readonly string _connectionString;
        public TaskStatusRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<TaskStatus> GetAllStatuses()
        {
            var statuses = new List<TaskStatus>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            // Обратите внимание: имя столбца должно соответствовать базе (например, status_name)
            var cmd = new SQLiteCommand("SELECT id, status_name FROM TaskStatus", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                statuses.Add(new TaskStatus
                {
                    Id = Convert.ToInt32(reader["id"]),
                    StatusName = reader["status_name"].ToString() ?? ""
                });
            }
            return statuses;
        }
    }
}