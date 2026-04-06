using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.Json;
using HackatonUi.Models;

namespace HackatonUi.Repositories;

public class BuildingAttributeRepository
{
    private readonly List<BuildingAttribute> _attributes = new();
    private readonly string _connectionString;

    public BuildingAttributeRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public void UpdateAttribute(BuildingAttribute attr)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
        UPDATE BuildingAttributes
        SET Section = @section, Key = @key, Value = @value
        WHERE id = @id
    ", conn);

        cmd.Parameters.AddWithValue("@id", attr.Id);
        cmd.Parameters.AddWithValue("@section", attr.Section);
        cmd.Parameters.AddWithValue("@key", attr.Key);
        cmd.Parameters.AddWithValue("@value", attr.Value);

        cmd.ExecuteNonQuery();
    }

    public void AddAttribute(BuildingAttribute attr)
    {
        using var conec = new SQLiteConnection(_connectionString);
        conec.Open();
        var cmd = new SQLiteCommand(@"
            INSERT INTO BuildingAttributes (building_id, section, key, value)
            VALUES (@bid, @section, @key, @value)
        ", conec);
        cmd.Parameters.AddWithValue("@bid", attr.BuildingId);
        cmd.Parameters.AddWithValue("@section", attr.Section);
        cmd.Parameters.AddWithValue("@key", attr.Key);
        cmd.Parameters.AddWithValue("@value", attr.Value);
        cmd.ExecuteNonQuery();
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
                BuildingId = Convert.ToInt32(reader["building_id"]),
                Section = reader["section"].ToString()!,
                Key = reader["key"].ToString()!,
                Value = reader["value"].ToString()!
            });
        }
        return list;
    }

    

    public List<BuildingAttribute> GetAll() => _attributes;
}