using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HackatonUi.Models;
using HackatonUi.Data;
using HackatonUi.Models;
using HackatonUi.Repositories;
using HackatonUi.ViewModels;

class Program
{
    static void MainCon()
    {
        var connectionString =
            "Data Source=/home/egor/Рабочий стол/DEV/2kurs/C#/Rider/ConsoleApp1/ConsoleApp1/hackaton.db;Version=3;";
        DatabaseInitializer.Initialize(connectionString);
        var mainVM = new MainWindowViewModel();
        var userRepo = new UserRepository(connectionString);
        var buildingRepo = new BuildingRepository(connectionString);
        var docRepo = new BuildingDocumentRepository(connectionString);
        var controlDateRepo = new ControlDateRepository(connectionString);
        var attributeRepo = new BuildingAttributeRepository(connectionString);


        UserCredentials? currentUser = null;

        while (true)
        {
            if (currentUser == null)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Регистрация");
                Console.WriteLine("2 - Вход");
                Console.WriteLine("0 - Выход");
                Console.Write(">>> ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Имя пользователя: ");
                        var ru = Console.ReadLine();
                        Console.Write("Пароль: ");
                        var rp = Console.ReadLine();
                        Console.Write("Роль (Admin/Expert/Viewer): ");
                        var rr = Console.ReadLine() ?? "Viewer";

                        var registered = mainVM.Register(ru!, rp!, rr);
                        Console.WriteLine(registered ? "✅ Успешная регистрация" : "⚠️ Пользователь уже существует");
                        break;

                    case "2":
                        Console.Write("Имя пользователя: ");
                        var lu = Console.ReadLine();
                        Console.Write("Пароль: ");
                        var lp = Console.ReadLine();
                        var logged = mainVM.Login(lu!, lp!);
                        if (logged != null)
                        {
                            Console.WriteLine($"✅ Вход выполнен, роль: {logged.RoleName}");
                            currentUser = logged;
                        }
                        else
                        {
                            Console.WriteLine("❌ Неверные данные");
                        }

                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("❓ Неизвестный выбор");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"\nПривет, {currentUser.Username}! Ваша роль: {currentUser.RoleName}");
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1  -  Добавить здание");
                Console.WriteLine("2  -  Добавить документ к зданию");
                Console.WriteLine("3  -  Добавить контрольную дату к зданию");
                Console.WriteLine("4  -  Просмотреть контрольные даты здания");
                Console.WriteLine("5  -  Загрузить здание из XML-файла");
                Console.WriteLine("6  -  Ручное добавление здания");
                Console.WriteLine("7  -  Редактирование объекта");
                Console.WriteLine("8  -  Удаление объекта");
                Console.WriteLine("9  -  Добавить задачу к зданию");
                Console.WriteLine("10 -  Добавить решение по задаче");
                Console.WriteLine("11 -  Посмотреть статус задачи");
                Console.WriteLine("12 -  Изменить статус задачи");
                Console.WriteLine("13 -  Выйти");
                Console.Write(">>> ");
                var action = Console.ReadLine();
                
                switch (action)
                {
                    case "1":
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("❌ Нет прав для добавления здания");
                            break;
                        }

                        Console.Write("Адрес здания: ");
                        var addr = Console.ReadLine();
                        Console.Write("Кадастровый номер :");
                        var cadadr = Console.ReadLine();
                        Console.Write("Количество этажей :");
                        var floor = Console.ReadLine();
                        Console.Write("Общая площадь :");
                        var area = Console.ReadLine();
                        Console.Write("Тип здания :");
                        var buildtype = Console.ReadLine();
                        var building = new Building
                        {
                            Address = addr!,
                            CadastralNumber = Convert.ToInt32(cadadr),
                            Floors = Convert.ToInt32(floor),
                            Area = Convert.ToDouble(area),
                            BuildingTypeId = Convert.ToInt32(buildtype!),
                        };
                        buildingRepo.Add(building);
                        Console.WriteLine("✅ Здание добавлено");
                        break;

                    case "2":
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("❌ Нет прав для добавления документа");
                            break;
                        }

                        Console.Write("ID здания для документа: ");
                        if (!int.TryParse(Console.ReadLine(), out var bIdDoc))
                        {
                            Console.WriteLine("❌ Некорректный ID");
                            break;
                        }

                        Console.Write("Путь к файлу: ");
                        var path = Console.ReadLine();
                        var doc = new BuildingDocument
                        {
                            BuildingId = bIdDoc,
                            FilePath = path!,
                            UploadedAt = DateTime.UtcNow,
                            UploadedBy = currentUser.Username
                        };
                        docRepo.AddDocument(doc);
                        Console.WriteLine("✅ Документ добавлен");
                        break;

                    case "3":
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("❌ Нет прав для добавления контрольной даты");
                            break;
                        }

                        Console.Write("ID здания для контрольной даты: ");
                        if (!int.TryParse(Console.ReadLine(), out var bIdDate))
                        {
                            Console.WriteLine("❌ Некорректный ID");
                            break;
                        }

                        Console.Write("Название контрольной даты: ");
                        var title = Console.ReadLine();
                        Console.Write("Дата (ГГГГ-ММ-ДД): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out var due))
                        {
                            Console.WriteLine("❌ Некорректная дата");
                            break;
                        }

                        var cd3 = new ControlDate
                        {
                            BuildingId = bIdDate,
                            Title = title!,
                            DueDate = due,
                            IsDone = false
                        };
                        controlDateRepo.AddControlDate(cd3);
                        Console.WriteLine("✅ Контрольная дата добавлена");
                        break;

                    case "4":
                        Console.Write("ID здания для просмотра дат: ");
                        if (!int.TryParse(Console.ReadLine(), out var bIdView))
                        {
                            Console.WriteLine("❌ Некорректный ID");
                            break;
                        }

                        var dates = controlDateRepo.GetControlDates(bIdView);
                        if (dates.Count == 0)
                        {
                            Console.WriteLine("❗ Контрольных дат нет");
                            break;
                        }

                        foreach (var d in dates)
                        {
                            Console.WriteLine(
                                $"[{d.Id}] {d.Title} - {d.DueDate:yyyy-MM-dd} {(d.IsDone ? "(Выполнено)" : "(Не выполнено)")}");
                        }

                        break;
                    case "5":
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("❌ Нет прав");
                            break;
                        }

                        Console.Write("Путь к XML-файлу: ");
                        var pathXml = Console.ReadLine();

                        if (!File.Exists(pathXml))
                        {
                            Console.WriteLine("❌ Файл не найден");
                            break;
                        }

                        try
                        {
                            var buildings = LoadBuildingsFromXml(pathXml!, out var docs, out var controlDates);

                            foreach (var b in buildings)
                            {
                                buildingRepo.Add(b);
                                var realId = b.Id;

                                foreach (var attr in b.Attributes)
                                {
                                    attr.BuildingId = realId;
                                    attributeRepo.AddAttribute(attr);
                                }

                                foreach (var d in docs.Where(x => x.BuildingId == buildings.IndexOf(b) + 1))
                                {
                                    d.BuildingId = realId;
                                    docRepo.AddDocument(d);
                                }

                                foreach (var cd in controlDates.Where(x => x.BuildingId == buildings.IndexOf(b) + 1))
                                {
                                    cd.BuildingId = realId;
                                    controlDateRepo.AddControlDate(cd);
                                }
                            }


                            Console.WriteLine($"✅ Загружено зданий: {buildings.Count}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Ошибка загрузки: " + ex.Message);
                        }

                        break;
                    case "6":
                        DatabaseInitializer.Initialize(connectionString);
                        AddBuildingManually(connectionString);
                        break;
                    case "7":
                        DatabaseInitializer.Initialize(connectionString);
                        EditBuildingManually(connectionString);
                        break;
                    case "8":
                        DatabaseInitializer.Initialize(connectionString);
                        Console.Write("Введите ID здания для удаления: ");
                        int id = int.Parse(Console.ReadLine()!);
                        DeleteBuilding(id, connectionString );
                        break;
                    case "9":
                        DatabaseInitializer.Initialize(connectionString);
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("❌ Нет прав для добавления задачи к зданию");
                            break;
                        }
                        AddTaskToBuilding(connectionString);
                        break;
                    case "10":
                        DatabaseInitializer.Initialize(connectionString);
                        if (!HasRole(currentUser, "Admin", "Expert"))
                        {
                            Console.WriteLine("@{❌ Нет прав для добавления  решения по задачам");
                            break;
                        }
                        AddSolutionToTask(connectionString);

                        break;
                    case "11":
                        // Посмотреть статус задачи
                        DatabaseInitializer.Initialize(connectionString);
                        ViewTaskStatus(connectionString);
                        break;
                    case "12":
                          // Изменить статус задачи
                          DatabaseInitializer.Initialize(connectionString);
                          if (!HasRole(currentUser, "Admin", "Expert"))
                          {
                              Console.WriteLine("@{❌ Нет прав для изменения статуса решения задач");
                              break;
                          }
                          UpdateTaskStatus(connectionString);
                        break;
        
                    case "13":
                        currentUser = null;
                        Console.WriteLine("Выход выполнен");
                        break;

                    default:
                        Console.WriteLine("❓ Неизвестное действие");
                        break;
                }
            }
        }
    }

    static bool HasRole(UserCredentials user, params string[] roles)
    {
        if (user.RoleName == null) return false;
        foreach (var role in roles)
            if (user.RoleName.Equals(role, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    static List<Building> LoadBuildingsFromXml(string filePath,
        out List<BuildingDocument> documents,
        out List<ControlDate> controlDates)
    {
        var serializer = new XmlSerializer(typeof(BuildingXmlList));
        using var stream = new FileStream(filePath, FileMode.Open);
        var xmlData = (BuildingXmlList)serializer.Deserialize(stream)!;

        var result = new List<Building>();
        documents = new List<BuildingDocument>();
        controlDates = new List<ControlDate>();

        foreach (var dto in xmlData.Items)
        {
            var building = new Building
            {
                Address = dto.Address,
                CadastralNumber = dto.CadastralNumber,
                Floors = dto.Floors,
                Area = dto.Area,
                BuildingTypeId = dto.BuildingTypeId,
                Attributes = dto.Attributes.Select(attr => new BuildingAttribute
                {
                    Section = attr.Section,
                    Key = attr.Key,
                    Value = attr.Value
                }).ToList()
            };

            result.Add(building);

            int currentId = result.Count; // временно как ID-связь

            documents.AddRange(dto.Documents.Select(doc => new BuildingDocument
            {
                BuildingId = currentId, // заменишь на правильный ID после сохранения
                FilePath = doc.Path,
                UploadedBy = doc.Uploader,
                UploadedAt = doc.Date
            }));

            controlDates.AddRange(dto.ControlDates.Select(cd => new ControlDate
            {
                BuildingId = currentId,
                Title = cd.Title,
                DueDate = cd.DueDate,
                IsDone = false
            }));
        }

        return result;
    }

    static void AddBuildingManually(string connectionString)
    {
        DatabaseInitializer.Initialize(connectionString);

        var userRepo = new UserRepository(connectionString);
        var buildingRepo = new BuildingRepository(connectionString);
        var docRepo = new BuildingDocumentRepository(connectionString);
        var controlDateRepo = new ControlDateRepository(connectionString);
        var attributeRepo = new BuildingAttributeRepository(connectionString);

        Console.WriteLine("🔧 Добавление нового здания вручную:");

        Console.Write("Адрес: ");
        string address = Console.ReadLine()!;

        Console.Write("Кадастровый номер: ");
        int cadastral = int.Parse(Console.ReadLine()!);

        Console.Write("Количество этажей: ");
        int floors = int.Parse(Console.ReadLine()!);

        Console.Write("Площадь: ");
        double area = double.Parse(Console.ReadLine()!);

        Console.Write("ID типа здания: ");
        int typeId = int.Parse(Console.ReadLine()!);

        var building = new Building
        {
            Address = address,
            CadastralNumber = cadastral,
            Floors = floors,
            Area = area,
            BuildingTypeId = typeId
        };

        buildingRepo.Add(building);

        int buildingId = building.Id;

        // Добавление атрибутов
        Console.WriteLine("➕ Добавим атрибуты (введите пустой раздел чтобы закончить):");
        while (true)
        {
            Console.Write("Раздел: ");
            string section = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(section)) break;

            Console.Write("Ключ: ");
            string key = Console.ReadLine()!;

            Console.Write("Значение: ");
            string value = Console.ReadLine()!;

            var attr = new BuildingAttribute
            {
                BuildingId = buildingId,
                Section = section,
                Key = key,
                Value = value
            };

            attributeRepo.AddAttribute(attr);
        }

        // Добавление документов
        Console.WriteLine("📄 Добавим документы (пустой путь чтобы закончить):");
        while (true)
        {
            Console.Write("Путь к файлу: ");
            string path = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(path)) break;

            Console.Write("Кто загрузил: ");
            string uploader = Console.ReadLine()!;

            Console.Write("Дата загрузки (гггг-мм-дд): ");
            DateTime date = DateTime.Parse(Console.ReadLine()!);

            var doc = new BuildingDocument
            {
                BuildingId = buildingId,
                FilePath = path,
                UploadedBy = uploader,
                UploadedAt = date
            };

            docRepo.AddDocument(doc);
        }

        // Добавление контрольных дат
        Console.WriteLine("📅 Добавим контрольные даты (пустое название чтобы закончить):");
        while (true)
        {
            Console.Write("Название: ");
            string title = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(title)) break;

            Console.Write("Срок (гггг-мм-дд): ");
            DateTime dueDate = DateTime.Parse(Console.ReadLine()!);

            var controlDate = new ControlDate
            {
                BuildingId = buildingId,
                Title = title,
                DueDate = dueDate,
                IsDone = false
            };

            controlDateRepo.AddControlDate(controlDate);
        }

        Console.WriteLine("✅ Здание успешно добавлено.");
    }

    static void EditBuildingManually(string connectionString)
    {
        var buildingRepo = new BuildingRepository(connectionString);
        var attrRepo = new BuildingAttributeRepository(connectionString);
        var docRepo = new BuildingDocumentRepository(connectionString);

        Console.Write("Введите ID здания для редактирования: ");
        if (!int.TryParse(Console.ReadLine(), out int buildingId))
        {
            Console.WriteLine("❌ Неверный ID");
            return;
        }

        var buildings = buildingRepo.Search(""); // Получаем все
        var building = buildings.FirstOrDefault(b => b.Id == buildingId);
        if (building == null)
        {
            Console.WriteLine("❌ Здание не найдено");
            return;
        }

        // Редактирование основных полей
        Console.WriteLine($"Редактирование здания: {building.Address}");

        Console.Write($"Новый адрес (было: {building.Address}): ");
        var newAddress = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAddress))
            building.Address = newAddress;

        Console.Write($"Новый кадастровый номер (было: {building.CadastralNumber}): ");
        var cad = Console.ReadLine();
        if (int.TryParse(cad, out int newCad))
            building.CadastralNumber = newCad;

        Console.Write($"Новая этажность (было: {building.Floors}): ");
        var fl = Console.ReadLine();
        if (int.TryParse(fl, out int newFl))
            building.Floors = newFl;

        Console.Write($"Новая площадь (было: {building.Area}): ");
        var ar = Console.ReadLine();
        if (double.TryParse(ar, out double newArea))
            building.Area = newArea;

        Console.Write($"Новый тип здания (было: {building.BuildingTypeId}): ");
        var type = Console.ReadLine();
        if (int.TryParse(type, out int newType))
            building.BuildingTypeId = newType;

        buildingRepo.Update(building);
        Console.WriteLine("✅ Основные данные обновлены");

        // Редактирование атрибутов
        var attributes = attrRepo.GetByBuildingId(buildingId);
        foreach (var attr in attributes)
        {
            Console.WriteLine($"Атрибут: {attr.Section} / {attr.Key} = {attr.Value}");
            Console.Write("Введите новое значение (или Enter для пропуска): ");
            var val = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(val))
            {
                attr.Value = val;
                attrRepo.UpdateAttribute(attr);
                Console.WriteLine("✅ Обновлено");
            }
        }
        var docs = docRepo.GetDocuments(buildingId);
        foreach (var doc in docs)
        {
            Console.WriteLine($"\nДокумент ID: {doc.Id}");
            Console.WriteLine($"Путь: {doc.FilePath}");
            Console.WriteLine($"Загружено: {doc.UploadedAt} пользователем: {doc.UploadedBy}");

            Console.Write("Новый путь к файлу (Enter чтобы не менять): ");
            var newPath = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newPath))
                doc.FilePath = newPath;

            Console.Write("Новый пользователь (Enter чтобы не менять): ");
            var newUser = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newUser))
                doc.UploadedBy = newUser;

            // Обновляем документ
            docRepo.UpdateDocument(doc);
            Console.WriteLine("✅ Документ обновлён.");
        }
    }

    static void DeleteBuilding(int buildingId, string connectionString)
    {
        DatabaseInitializer.Initialize(connectionString);

        var buildingRepo = new BuildingRepository(connectionString);

        // Удаляем связанные атрибуты
        using (var conn = new SQLiteConnection(connectionString))
        {
            conn.Open();

            var attrCmd = new SQLiteCommand("DELETE FROM BuildingAttributes WHERE BuildingId = @id", conn);
            attrCmd.Parameters.AddWithValue("@id", buildingId);
            attrCmd.ExecuteNonQuery();

            var docCmd = new SQLiteCommand("DELETE FROM BuildingDocument WHERE building_id = @id", conn);
            docCmd.Parameters.AddWithValue("@id", buildingId);
            docCmd.ExecuteNonQuery();

            var dateCmd = new SQLiteCommand("DELETE FROM ControlDate WHERE building_id = @id", conn);
            dateCmd.Parameters.AddWithValue("@id", buildingId);
            dateCmd.ExecuteNonQuery();
        }

        // Удаляем само здание
        buildingRepo.Delete(buildingId);

        Console.WriteLine($"Здание с ID {buildingId} и все связанные данные удалены.");
    }
    //"Добавить задачу к зданию"
    static void AddTaskToBuilding(string connectionString)
    {
        DatabaseInitializer.Initialize(connectionString);
        
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
    //Добавить решение по задаче
    static void AddSolutionToTask(string connectionString )
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
            INSERT INTO TaskDecision (description, status_id, task_id)
            VALUES (@desc, @status, @taskId)";
            command.Parameters.AddWithValue("@desc", solutionDesc);
            command.Parameters.AddWithValue("@status", statusId);
            command.Parameters.AddWithValue("@taskId", taskId);

            command.ExecuteNonQuery();
            Console.WriteLine("Решение добавлено.");
        }
    }
    //Посмотреть статус задачи
    static void ViewTaskStatus(string connectionString)
    {
        Console.Write("Введите ID задачи: ");
        if (!int.TryParse(Console.ReadLine(), out int taskId))
        {
            Console.WriteLine("Неверный ID задачи.");
            return;
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
    }


    static void UpdateTaskStatus(string connectionString)
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

