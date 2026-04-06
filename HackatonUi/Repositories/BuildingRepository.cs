using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using HackatonUi.Models;

namespace HackatonUi.Repositories;

public class BuildingRepository
{
    private readonly string _connectionString;
    public BuildingRepository(string connectionString) => _connectionString = connectionString;

    public void Add(Building b)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("INSERT INTO Building (address, cadastral_number, floors, area, building_type_id) VALUES (@a, @c, @f, @ar, @bt)", conn);
        cmd.Parameters.AddWithValue("@a", b.Address);
        cmd.Parameters.AddWithValue("@c", b.CadastralNumber);
        cmd.Parameters.AddWithValue("@f", b.Floors);
        cmd.Parameters.AddWithValue("@ar", b.Area);
        cmd.Parameters.AddWithValue("@bt", b.BuildingTypeId);
        cmd.ExecuteNonQuery();
        cmd = new SQLiteCommand("SELECT last_insert_rowid()", conn);
        b.Id = (int)(long)cmd.ExecuteScalar();  
    }

    public void Delete(int id)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("DELETE FROM Building WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public void Update(Building b)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("UPDATE Building SET address = @a, cadastral_number = @c, floors = @f, area = @ar, building_type_id = @bt WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", b.Id);
        cmd.Parameters.AddWithValue("@a", b.Address);
        cmd.Parameters.AddWithValue("@c", b.CadastralNumber);
        cmd.Parameters.AddWithValue("@f", b.Floors);
        cmd.Parameters.AddWithValue("@ar", b.Area);
        cmd.Parameters.AddWithValue("@bt", b.BuildingTypeId);
        cmd.ExecuteNonQuery();
    }

    public List<Building> Search(string keyword)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("SELECT * FROM Building WHERE address LIKE @k", conn);
        cmd.Parameters.AddWithValue("@k", $"%{keyword}%");
        using var reader = cmd.ExecuteReader();
        var list = new List<Building>();
        while (reader.Read())
        {
            list.Add(new Building
            {
                Id = Convert.ToInt32(reader["id"]),
                Address = reader["address"].ToString()!,
                CadastralNumber = Convert.ToInt32(reader["cadastral_number"]),
                Floors = Convert.ToInt32(reader["floors"]),
                Area = Convert.ToDouble(reader["area"]),
                BuildingTypeId = Convert.ToInt32(reader["building_type_id"])
            });
        }

        return list;
    }
    public void AddAttribute(int buildingId, string section, string key, string value)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        connection.Execute("INSERT INTO BuildingAttributes (BuildingId, Section, Key, Value) VALUES (@buildingId, @section, @key, @value)",
            new { buildingId, section, key, value });
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

    public List<BuildingAttribute> GetByBuildingId(int buildingId)
    {
        var list = new List<BuildingAttribute>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand("SELECT * FROM BuildingAttributes WHERE BuildingId = @bid", conn);
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

    

    public List<BuildingAttribute> GetAttributes(int buildingId)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        return connection.Query<BuildingAttribute>(
            "SELECT * FROM BuildingAttributes WHERE BuildingId = @buildingId",
            new { buildingId }).ToList();
    }

    public List<Building> SearchByAttribute(string key, string value)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var buildings = connection.Query<Building>(
            @"SELECT DISTINCT b.* FROM Building b
          JOIN BuildingAttributes a ON b.Id = a.BuildingId
          WHERE a.Key = @key AND a.Value LIKE @value",
            new { key, value = $"%{value}%" }).ToList();

        foreach (var b in buildings)
            b.Attributes = GetAttributes(b.Id);

        return buildings;
    }
    

}