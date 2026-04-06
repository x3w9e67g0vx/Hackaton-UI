using System;
using System.Collections.Generic;
using System.Data.SQLite;
using HackatonUi.Data;
using HackatonUi.Models;
using HackatonUi.Data;

namespace HackatonUi.Repositories;

public class UserRepository
{
    

    private static string _connectionString;
    
    public UserRepository(string connectionString) => _connectionString = connectionString;

    // Регистрация с выбором роли по имени роли (например, "Admin", "Expert", "Viewer")
    public  bool Register(string username, string password, string roleName = "Viewer")
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM UserCredentials WHERE username = @u", conn);
        checkCmd.Parameters.AddWithValue("@u", username);
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
        if (exists) return false;

        // Получить id роли по имени
        var roleCmd = new SQLiteCommand("SELECT id FROM Role WHERE name = @name", conn);
        roleCmd.Parameters.AddWithValue("@name", roleName);
        var roleIdObj = roleCmd.ExecuteScalar();
        int roleId = roleIdObj != null ? Convert.ToInt32(roleIdObj) : 3; // 3 - Viewer по умолчанию

        var cmd = new SQLiteCommand("INSERT INTO UserCredentials (username, password, role_id) VALUES (@u, @p, @r)", conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        cmd.Parameters.AddWithValue("@r", roleId);
        cmd.ExecuteNonQuery();
        return true;
    }

    // Вход возвращает пользователя с ролью
    public  UserCredentials Login(string username, string password)
    {
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand(@"
        SELECT uc.id, uc.username, uc.password, uc.role_id, r.name as role_name
        FROM UserCredentials uc
        LEFT JOIN Role r ON uc.role_id = r.id
        WHERE uc.username = @u AND uc.password = @p
    ", conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new UserCredentials
            {
                Id = Convert.ToInt32(reader["id"]),
                Username = reader["username"].ToString()!,
                Password = reader["password"].ToString()!,
                RoleId = Convert.ToInt32(reader["role_id"]),
                RoleName = reader["role_name"].ToString()
            };
        }

        return null;
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
    public static List<string> GetAllRoleId()
    {
        var roles = new List<string>();
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();

        var cmd = new SQLiteCommand("SELECT name FROM Role", conn); // Исправлено: выбираем name, а не id
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            roles.Add(reader.GetString(0));
        }
        return roles;
    }
    public List<UserCredentials> GetUsersByRole(string role)
    {
        var users = new List<UserCredentials>();
    
        // Задайте соответствие строки и role_id.
        // Например, "viewer" соответствует role_id = 3.
        int roleId;
        if(role.ToLower() == "viewer")
        {
            roleId = 3;  // Измените это значение в соответствии с вашей системой.
        }
        else
        {
            // Для других ролей можно добавить свои условия или вернуть пустой список.
            roleId = -1; 
        }
    
        if(roleId < 0)
            return users; // ничего не найдено, так как роль не распознана.
    
        using var conn = new SQLiteConnection(_connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("SELECT * FROM UserCredentials WHERE role_id = @roleId", conn);
        cmd.Parameters.AddWithValue("@roleId", roleId);
    
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new UserCredentials
            {
                Id = Convert.ToInt32(reader["id"]),
                Username = reader["username"].ToString()!,
                // Здесь можно установить RoleName, если нужно, или оставить пустым.
                RoleName = role  
            });
        }
        return users;
    }


}
