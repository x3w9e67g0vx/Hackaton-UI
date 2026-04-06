using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories
{
    public class DecisionAssignmentRepository
    {
        private readonly string _connectionString;
        public DecisionAssignmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public bool AssignUserToDecision(int decisionId, int userId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var checkCmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM DecisionAssignments WHERE decision_id = @decisionId AND user_id = @userId", 
                conn);
            checkCmd.Parameters.AddWithValue("@decisionId", decisionId);
            checkCmd.Parameters.AddWithValue("@userId", userId);
            long count = (long)checkCmd.ExecuteScalar();
            if (count > 0)
            {
                return false; // Назначение уже существует.
            }

            var cmd = new SQLiteCommand(
                "INSERT INTO DecisionAssignments (decision_id, user_id) VALUES (@decisionId, @userId);", 
                conn);
            cmd.Parameters.AddWithValue("@decisionId", decisionId);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
            return true;
        }



        
        public List<DecisionAssignmentInfo> GetAllAssignments()
        {
            var list = new List<DecisionAssignmentInfo>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            // Здесь выполняем объединение таблиц:
            // – DecisionAssignments (da)
            // – TaskDecision (td) для информации о решении
            // – InspectionTasks (it) чтобы получить название задания, связанного с решением (td.task_id)
            // – usercredential (uc) для имени пользователя
            string query = @"
               SELECT da.id, da.decision_id, da.user_id, da.assigned_at,
                      td.description AS DecisionDescription, 
                      it.description AS TaskDescription,
                      uc.username
               FROM DecisionAssignments da
               JOIN TaskDecision td ON td.id = da.decision_id
               JOIN InspectionTask it ON it.id = td.task_id
               JOIN UserCredentials uc ON uc.id = da.user_id;
            ";
            using var cmd = new SQLiteCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DecisionAssignmentInfo
                {
                    Id = Convert.ToInt32(reader["id"]),
                    DecisionId = Convert.ToInt32(reader["decision_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    AssignedAt = reader["assigned_at"].ToString() ?? "",
                    DecisionDescription = reader["DecisionDescription"].ToString() ?? "",
                    TaskDescription = reader["TaskDescription"].ToString() ?? "",
                    Username = reader["username"].ToString() ?? ""
                });
            }
            return list;
        }
        public void DeleteAssignment(int assignmentId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("DELETE FROM DecisionAssignments WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", assignmentId);
            cmd.ExecuteNonQuery();
        }

    }
}
