using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories;
public class TaskDecisionRepository
    {
        private readonly string _connectionString;
        
        public TaskDecisionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        // Возвращает решения по ID задачи
        public List<TaskDecision> GetDecisionsByTaskId(int taskId)
        {
            var decisions = new List<TaskDecision>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("SELECT * FROM TaskDecision WHERE task_id = @tid", conn);
            cmd.Parameters.AddWithValue("@tid", taskId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                decisions.Add(new TaskDecision
                {
                    Id = Convert.ToInt32(reader["id"]),
                    TaskId = Convert.ToInt32(reader["task_id"]),
                    Description = reader["description"].ToString()!,
                    StatusId = Convert.ToInt32(reader["status_id"])
                });
            }
            return decisions;
        }
        
        // Добавляет решение
        public void AddDecision(TaskDecision decision)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand(@"
                INSERT INTO TaskDecision (task_id, description, status_id)
                VALUES (@tid, @desc, @status)", conn);
            cmd.Parameters.AddWithValue("@tid", decision.TaskId);
            cmd.Parameters.AddWithValue("@desc", decision.Description);
            cmd.Parameters.AddWithValue("@status", decision.StatusId);
            cmd.ExecuteNonQuery();
        }
        
        // Удаляет решение по ID
        public void DeleteDecision(int decisionId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("DELETE FROM TaskDecision WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", decisionId);
            cmd.ExecuteNonQuery();
        }
        public void UpdateDecision(TaskDecision decision)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var cmd = new SQLiteCommand(@"
        UPDATE TaskDecision
        SET description = @desc,
            status_id = @statusId
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@id", decision.Id);
            cmd.Parameters.AddWithValue("@desc", decision.Description);
            cmd.Parameters.AddWithValue("@statusId", decision.StatusId);

            cmd.ExecuteNonQuery();
        }

        
        public List<TaskDecision> GetAllDecisions()
        {
            var list = new List<TaskDecision>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("SELECT * FROM TaskDecision", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TaskDecision
                {
                    Id = Convert.ToInt32(reader["id"]),
                    TaskId = Convert.ToInt32(reader["task_id"]),
                    Description = reader["description"].ToString()!,
                    StatusId = Convert.ToInt32(reader["status_id"])
                });
            }
            return list;
        }
        
    }