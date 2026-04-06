using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories;

public class ControlDateRepository
{
    private readonly string _connectionString;
    public ControlDateRepository(string connectionString) => _connectionString = connectionString;

    public void AddControlDate(ControlDate date)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
            INSERT INTO ControlDate (building_id, title, due_date, is_done)
            VALUES (@bid, @title, @due, @done)
        ", conn);

        cmd.Parameters.AddWithValue("@bid", date.BuildingId);
        cmd.Parameters.AddWithValue("@title", date.Title);
        cmd.Parameters.AddWithValue("@due", date.DueDate.ToString("o"));
        cmd.Parameters.AddWithValue("@done", date.IsDone ? 1 : 0);

        cmd.ExecuteNonQuery();
    }

    public void UpdateControlDate(ControlDate date)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
            UPDATE ControlDate SET title = @title, due_date = @due, is_done = @done
            WHERE id = @id
        ", conn);

        cmd.Parameters.AddWithValue("@id", date.Id);
        cmd.Parameters.AddWithValue("@title", date.Title);
        cmd.Parameters.AddWithValue("@due", date.DueDate.ToString("o"));
        cmd.Parameters.AddWithValue("@done", date.IsDone ? 1 : 0);

        cmd.ExecuteNonQuery();
    }

    public List<ControlDate> GetControlDates(int buildingId)
    {
        var list = new List<ControlDate>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand("SELECT * FROM ControlDate WHERE building_id = @bid", conn);
        cmd.Parameters.AddWithValue("@bid", buildingId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new ControlDate
            {
                Id = Convert.ToInt32(reader["id"]),
                BuildingId = Convert.ToInt32(reader["building_id"]),
                Title = reader["title"].ToString()!,
                DueDate = DateTime.Parse(reader["due_date"].ToString()!),
                IsDone = Convert.ToInt32(reader["is_done"]) != 0
            });
        }
        return list;
    }
    public List<BuildingAttribute> GetByBuildingId(int buildingId)
    {
        var list = new List<BuildingAttribute>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand("SELECT * FROM BuildingAttributes WHERE building_id = @bid", conn);
        cmd.Parameters.AddWithValue("@bid", buildingId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new BuildingAttribute
            {
                Id = Convert.ToInt32(reader["id"]),
                BuildingId = Convert.ToInt32(reader["BuildingId"]),
                Section = reader["Section"].ToString()!,
                Key = reader["Key"].ToString()!,
                Value = reader["Value"].ToString()!
            });
        }
        return list;
    }
}

