using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Models;

namespace HackatonUi.Repositories;

public class BuildingDocumentRepository
{
    private readonly string _connectionString;
    public BuildingDocumentRepository(string connectionString) => _connectionString = connectionString;

    public void AddDocument(BuildingDocument doc)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
            INSERT INTO BuildingDocument (building_id, file_path, uploaded_at, uploaded_by)
            VALUES (@bid, @path, @date, @user)
        ", conn);

        cmd.Parameters.AddWithValue("@bid", doc.BuildingId);
        cmd.Parameters.AddWithValue("@path", doc.FilePath);
        cmd.Parameters.AddWithValue("@date", doc.UploadedAt.ToString("o"));
        cmd.Parameters.AddWithValue("@user", doc.UploadedBy);

        cmd.ExecuteNonQuery();
    }

    public List<BuildingDocument> GetDocuments(int buildingId)
    {
        var list = new List<BuildingDocument>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand("SELECT * FROM BuildingDocument WHERE building_id = @bid", conn);
        cmd.Parameters.AddWithValue("@bid", buildingId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new BuildingDocument
            {
                Id = Convert.ToInt32(reader["id"]),
                BuildingId = Convert.ToInt32(reader["building_id"]),
                FilePath = reader["file_path"].ToString()!,
                UploadedAt = DateTime.Parse(reader["uploaded_at"].ToString()!),
                UploadedBy = reader["uploaded_by"].ToString()!
            });
        }
        return list;
    }
    public void UpdateDocument(BuildingDocument doc)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
        UPDATE BuildingDocument 
        SET file_path = @path, uploaded_by = @user 
        WHERE id = @id
    ", conn);

        cmd.Parameters.AddWithValue("@id", doc.Id);
        cmd.Parameters.AddWithValue("@path", doc.FilePath);
        cmd.Parameters.AddWithValue("@user", doc.UploadedBy);

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
            list.Add(new BuildingAttribute()
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