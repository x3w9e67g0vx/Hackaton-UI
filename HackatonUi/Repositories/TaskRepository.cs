using System;
using System.Data.SQLite;
using HackatonUi.Data;

namespace HackatonUi.Repositories;

public class TaskRepository
{
    public static string connectionString => "Data Source=hackaton.db;Version=3;"; // Файл в папке приложения

    private static string _connectionString;
    
    public TaskRepository(string connectionString) => _connectionString = connectionString;

    public static void AddTaskToBuilding(string connectionString)
    {
        // DatabaseInitializer.Initialize(connectionString);
        
        Console.Write("Введите ID здания: ");
        int buildingId = int.Parse(Console.ReadLine());

        Console.Write("Введите описание задачи: ");
        string description = Console.ReadLine();

        Console.Write("Введите дату начала (yyyy-mm-dd): ");
        DateTime startDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Введите дату окончания (yyyy-mm-dd): ");
        DateTime endDate = DateTime.Parse(Console.ReadLine());

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO InspectionTask (description, building_id, start_date, end_date) 
                                VALUES (@desc, @bId, @start, @end)";
            command.Parameters.AddWithValue("@desc", description);
            command.Parameters.AddWithValue("@bId", buildingId);
            command.Parameters.AddWithValue("@start", startDate);
            command.Parameters.AddWithValue("@end", endDate);
            command.ExecuteNonQuery();
            Console.WriteLine("Задача добавлена.");
        }
    }

    public static void AddSolutionToTask(string connectionString )
    {
        DatabaseInitializer.Initialize(connectionString);
        
        Console.Write("Введите ID задачи: ");
        int taskId = int.Parse(Console.ReadLine());

        Console.Write("Введите описание решения: ");
        string solutionDesc = Console.ReadLine();

        Console.Write("Введите ID статуса: ");
        int statusId = int.Parse(Console.ReadLine());

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO TaskDecision (task_i,ddescription, status_id, )
            VALUES (@status, @desc,  @taskId)";
            command.Parameters.AddWithValue("@status", statusId);
            command.Parameters.AddWithValue("@desc", solutionDesc);
            command.Parameters.AddWithValue("@taskId", taskId);

            command.ExecuteNonQuery();
            Console.WriteLine("Решение добавлено.");
        }
    }

    public static string ViewTaskStatus(string connectionString)
    {
        Console.Write("Введите ID задачи: ");
        if (!int.TryParse(Console.ReadLine(), out int taskId))
        {
            Console.WriteLine("Неверный ID задачи.");
        }

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT s.status_name AS TaskStatus, td.description AS TaskDecision
            FROM TaskDecision td
            JOIN TaskStatus s ON td.id = s.id
            WHERE td.task_id = @tId";

            command.Parameters.AddWithValue("@tId", taskId);

            using (var reader = command.ExecuteReader())
            {
                bool hasRows = false;
                while (reader.Read())
                {
                    hasRows = true;
                    Console.WriteLine($"Статус: {reader["TaskStatus"]} | Решение: {reader["TaskDecision"]}");
                }

                if (!hasRows)
                {
                    Console.WriteLine("Нет решений по указанной задаче.");
                }
            }
        }

        return null;
    }

    public static void UpdateTaskStatus(string connectionString)
    {
        DatabaseInitializer.Initialize(connectionString);
        Console.Write("Введите ID решения: ");
        int solutionId = int.Parse(Console.ReadLine());

        Console.Write("Введите новый ID статуса: ");
        int newStatusId = int.Parse(Console.ReadLine());

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"UPDATE TaskDecision SET status_id = @newStatus WHERE id = @solId";
            command.Parameters.AddWithValue("@newStatus", newStatusId);
            command.Parameters.AddWithValue("@solId", solutionId);
            command.ExecuteNonQuery();
            Console.WriteLine("Статус обновлен.");
        }
    }
}