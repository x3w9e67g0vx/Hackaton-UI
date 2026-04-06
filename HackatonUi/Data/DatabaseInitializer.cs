using System.CodeDom;
using System.Data.SQLite;

namespace HackatonUi.Data;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        
       var buildingTypeTable = @"
        CREATE TABLE IF NOT EXISTS BuildingType (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );                                                                                                          ";

    var roleTable = @"
        CREATE TABLE IF NOT EXISTS Role (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE
        );";

    var buildingTable = @"
        CREATE TABLE IF NOT EXISTS Building (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            address TEXT NOT NULL,
            cadastral_number INTEGER,
            floors INTEGER,
            area REAL,
            building_type_id INTEGER,
            FOREIGN KEY (building_type_id) REFERENCES BuildingType(id)
        );";

    var buildingAttributeTable = @"
        CREATE TABLE IF NOT EXISTS BuildingAttributes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            building_id INTEGER NOT NULL,
            section TEXT NOT NULL,
            key TEXT NOT NULL,
            value TEXT,
            FOREIGN KEY (building_id) REFERENCES Building(id)
        );";

    var buildingDocumentTable = @"
        CREATE TABLE IF NOT EXISTS BuildingDocument (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            building_id INTEGER NOT NULL,
            file_path TEXT NOT NULL,
            uploaded_at TEXT,
            uploaded_by TEXT,
            FOREIGN KEY (building_id) REFERENCES Building(id)
        );";

    var controlDateTable = @"
        CREATE TABLE IF NOT EXISTS ControlDate (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            building_id INTEGER NOT NULL,
            title TEXT NOT NULL,
            due_date TEXT,
            is_done INTEGER DEFAULT 0 CHECK(is_done IN (0, 1)),
            FOREIGN KEY (building_id) REFERENCES Building(id)
        );";

    var inspectionTaskTable = @"
        CREATE TABLE IF NOT EXISTS InspectionTask (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            description TEXT NOT NULL,
            building_id INTEGER NOT NULL,
            start_date TEXT,
            end_date TEXT,
            FOREIGN KEY (building_id) REFERENCES Building(id)
        );";

    var taskStatusTable = @"
        CREATE TABLE IF NOT EXISTS TaskStatus (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            status_name TEXT NOT NULL UNIQUE
        );";

    var taskDecisionTable = @"
        CREATE TABLE IF NOT EXISTS TaskDecision (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            task_id INTEGER NOT NULL,
            description TEXT NOT NULL,
            status_id INTEGER NOT NULL,
            FOREIGN KEY (task_id) REFERENCES InspectionTask(id),
            FOREIGN KEY (status_id) REFERENCES TaskStatus(id)
        );";

    var userCredentialsTable = @"
        CREATE TABLE IF NOT EXISTS UserCredentials (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT NOT NULL UNIQUE,
            password TEXT NOT NULL,
            role_id INTEGER NOT NULL,
            FOREIGN KEY (role_id) REFERENCES Role(id)
        );";
    var decisinAssigmentTable = @" 
    CREATE TABLE IF NOT EXISTS DecisionAssignments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        decision_id INTEGER NOT NULL,
        user_id INTEGER NOT NULL,
        assigned_at TEXT DEFAULT (datetime('now')),
        FOREIGN KEY(decision_id) REFERENCES TaskDecision(id),
        FOREIGN KEY(user_id) REFERENCES UserCredentials(id),
        UNIQUE(decision_id, user_id)
        );";
    

    // Заполнение начальными данными
    var insertBuildingTypes = @"
         INSERT OR IGNORE INTO BuildingType (name) VALUES 
        ('Жилой дом'),
        ('Торговый центр'),
        ('Промышленное здание');";

    var insertRoles = @"
         INSERT OR IGNORE INTO  Role (name) VALUES 
        ('Admin'),
        ('Expert'),
        ('Viewer');";

    var insertTaskStatuses = @"
         INSERT OR IGNORE INTO TaskStatus (status_name) VALUES 
        ('Не начато'),
        ('В процеcсе'),
        ('Завершен'),
        ('Просроченно');";


    using var cmd = new SQLiteCommand(connection);

    
    cmd.CommandText = buildingTypeTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = roleTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = taskStatusTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = buildingTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = buildingAttributeTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = buildingDocumentTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = controlDateTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = inspectionTaskTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = taskDecisionTable;
    cmd.ExecuteNonQuery();

    cmd.CommandText = userCredentialsTable;
    cmd.ExecuteNonQuery();
    
    cmd.CommandText = decisinAssigmentTable;
    cmd.ExecuteNonQuery();

    // Заполняем начальные данные
    cmd.CommandText = insertBuildingTypes;
    cmd.ExecuteNonQuery();

    cmd.CommandText = insertRoles;
    cmd.ExecuteNonQuery();

    cmd.CommandText = insertTaskStatuses;
    cmd.ExecuteNonQuery();

    }
}
