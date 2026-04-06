using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HackatonUi.Views
{
    public partial class EditBuildingWindow : Window
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private List<Building> _buildings = new();

        public EditBuildingWindow()
        {
            InitializeComponent();
            _buildingRepo = new BuildingRepository(_connectionString);
            LoadBuildings();
        }

        private void LoadBuildings()
        {
            _buildings = _buildingRepo.Search(""); // Загружаем все здания
            foreach (var building in _buildings)
            {
                BuildingComboBox.Items.Add($"ID {building.Id}: {building.Address}");
            }
        }

        private void BuildingSelected(object sender, SelectionChangedEventArgs e)
        {
            if (BuildingComboBox.SelectedIndex >= 0)
            {
                var selectedBuilding = _buildings[BuildingComboBox.SelectedIndex];
                
                AddressBox.Text = selectedBuilding.Address;
                CadastralBox.Text = selectedBuilding.CadastralNumber.ToString();
                FloorsBox.Text = selectedBuilding.Floors.ToString();
                AreaBox.Text = selectedBuilding.Area.ToString();
                TypeComboBox.SelectedIndex = selectedBuilding.BuildingTypeId - 1;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e) 
        {
            if (BuildingComboBox.SelectedIndex < 0) return;

            var building = _buildings[BuildingComboBox.SelectedIndex];

            building.Address = AddressBox.Text;
            building.CadastralNumber = int.TryParse(CadastralBox.Text, out int cadastral) ? cadastral : 0;
            building.Floors = int.TryParse(FloorsBox.Text, out int floors) ? floors : 0;
            building.Area = double.TryParse(AreaBox.Text, out double area) ? area : 0;
            building.BuildingTypeId = TypeComboBox.SelectedIndex + 1;

            _buildingRepo.Update(building);
    
            StatusText.Text = "✅ Здание обновлено!";
    
            await Task.Yield(); 
    
            await Task.Delay(2000);
    
            this.Close();
        }
    }
}
