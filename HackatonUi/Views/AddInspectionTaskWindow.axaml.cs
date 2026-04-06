using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.Views;
     public partial class AddInspectionTaskWindow : Window
    {
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly BuildingRepository _buildingRepo;
        private readonly InspectionTaskRepository _taskRepo;
        
        public AddInspectionTaskWindow()
        {
            InitializeComponent();
            _buildingRepo = new BuildingRepository(_connectionString);
            _taskRepo = new InspectionTaskRepository(_connectionString);
            LoadBuildings();
        }
        
        private void LoadBuildings()
        {
            // Получаем список зданий, например, методом Search("")
            var buildings = _buildingRepo.Search("");
            foreach(var b in buildings)
            {
                BuildingComboBox.Items.Add($"ID {b.Id}: {b.Address}");
            }
        }
        
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (BuildingComboBox.SelectedIndex < 0)
            {
                StatusText.Text = "Выберите здание.";
                return;
            }
            
            int buildingId = _buildingRepo.Search("").Find(b => 
                $"ID {b.Id}: {b.Address}" == BuildingComboBox.SelectedItem.ToString())?.Id ?? 0;
            
            if (buildingId == 0)
            {
                StatusText.Text = "Некорректное здание.";
                return;
            }
            
            var task = new InspectionTask
            {
                BuildingId = buildingId,
                Description = DescriptionBox.Text,
                StartDate = StartDatePicker.SelectedDate?.DateTime ?? DateTime.Now,
                EndDate = EndDatePicker.SelectedDate?.DateTime ?? DateTime.Now.AddDays(1)
            };
            
            try
            {
                _taskRepo.AddTask(task);
                StatusText.Text = "Задача успешно добавлена.";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
