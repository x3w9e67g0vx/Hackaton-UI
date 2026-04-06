using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories;
    public class InspectionTaskRepository
    {
        private readonly string _connectionString;
        
        public InspectionTaskRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public List<InspectionTask> GetAllTasks()
        {
            var list = new List<InspectionTask>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("SELECT * FROM InspectionTask", conn);
            using var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                list.Add(new InspectionTask
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString()!,
                    BuildingId = Convert.ToInt32(reader["building_id"]),
                    StartDate = DateTime.Parse(reader["start_date"].ToString()!),
                    EndDate = DateTime.Parse(reader["end_date"].ToString()!)
                });
            }
            return list;
        }
        
        public List<InspectionTask> GetTasksByBuildingId(int buildingId)
        {
            var list = new List<InspectionTask>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("SELECT * FROM InspectionTask WHERE building_id = @bid", conn);
            cmd.Parameters.AddWithValue("@bid", buildingId);
            using var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                list.Add(new InspectionTask
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Description = reader["description"].ToString()!,
                    BuildingId = Convert.ToInt32(reader["building_id"]),
                    StartDate = DateTime.Parse(reader["start_date"].ToString()!),
                    EndDate = DateTime.Parse(reader["end_date"].ToString()!)
                });
            }
            return list;
        }
        public void UpdateTask(InspectionTask task)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var cmd = new SQLiteCommand(@"
        UPDATE InspectionTask
        SET description = @desc,
            building_id = @buildingId,
            start_date = @startDate,
            end_date = @endDate
        WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@id", task.Id);
            cmd.Parameters.AddWithValue("@desc", task.Description);
            cmd.Parameters.AddWithValue("@buildingId", task.BuildingId);
            cmd.Parameters.AddWithValue("@startDate", task.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@endDate", task.EndDate.ToString("yyyy-MM-dd"));

            cmd.ExecuteNonQuery();
        }
        
        public void AddTask(InspectionTask task)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand(@"
                INSERT INTO InspectionTask (description, building_id,  start_date, end_date)
                VALUES (@desc, @bid,  @start, @end)
            ", conn);
            cmd.Parameters.AddWithValue("@desc", task.Description);
            cmd.Parameters.AddWithValue("@bid", task.BuildingId);
            cmd.Parameters.AddWithValue("@start", task.StartDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@end", task.EndDate.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();
        }
        
        public void DeleteTask(int taskId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            var cmd = new SQLiteCommand("DELETE FROM InspectionTask WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", taskId);
            cmd.ExecuteNonQuery();
        }
    }

