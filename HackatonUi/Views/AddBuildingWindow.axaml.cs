using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HackatonUi.Views
{
    public partial class AddBuildingWindow : Window
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";

        public AddBuildingWindow()
        {
            InitializeComponent();
            _buildingRepo = new BuildingRepository(_connectionString);
        }

        private async void Save_Click(object sender, RoutedEventArgs e) // Добавляем async
        {
            try
            {
                // Формируем новый объект Building из данных, введённых пользователем
                var building = new Building
                {
                    Address = AddressBox.Text,
                    CadastralNumber = int.TryParse(CadastralBox.Text, out int cadastral) ? cadastral : 0,
                    Floors = int.TryParse(FloorsBox.Text, out int floors) ? floors : 0,
                    Area = double.TryParse(AreaBox.Text, out double area) ? area : 0,
                    BuildingTypeId = 1 // Здесь указываем значение по умолчанию, можно расширить выбор через ComboBox
                };

                // Добавляем здание в базу
                _buildingRepo.Add(building);
        
                // Обновляем статус
                StatusText.Text = "Здание успешно добавлено!";
                
        
                // Принудительно обновляем UI
                await Task.Yield();
        
                // Ждем 3 секунды
                await Task.Delay(2000);
        
                this.Close();
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}