using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HackatonUi.Views
{
    public partial class DeleteBuildingWindow : Window
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private List<Building> _buildings = new();

        public DeleteBuildingWindow()
        {
            InitializeComponent();
            _buildingRepo = new BuildingRepository(_connectionString);
            LoadBuildings();
        }

        private void LoadBuildings()
        {
            _buildings = _buildingRepo.Search(""); 
            foreach (var building in _buildings)
            {
                BuildingComboBox.Items.Add($"ID {building.Id}: {building.Address}");
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BuildingComboBox.SelectedIndex < 0)
                {
                    StatusText.Text = "⚠️ Выберите здание для удаления.";
                    return;
                }

                var selectedBuilding = _buildings[BuildingComboBox.SelectedIndex];

                // Удаление здания
                _buildingRepo.Delete(selectedBuilding.Id);
        
                // Обновление UI
                BuildingComboBox.Items.RemoveAt(BuildingComboBox.SelectedIndex);
                _buildings.Remove(selectedBuilding);
        
                // Обновление статуса
                StatusText.Text = $"✅ Здание ID {selectedBuilding.Id} удалено.";
        
                // Принудительное обновление UI для отображения статуса
                await Task.Delay(1); // Короткая задержка для обработки UI сообщений
        
                // Ждем 2 секунды перед закрытием
                await Task.Delay(2000);
        
                this.Close();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ Ошибка при удалении: {ex.Message}";
            }
        }
    }
}