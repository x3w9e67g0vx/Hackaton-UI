using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;
using System.Collections.Generic;

namespace HackatonUi.Views
{
    public partial class AddAttributeWindow : Window
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly BuildingAttributeRepository _attributeRepo;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private List<Building> _buildings = new();

        public AddAttributeWindow()
        {
            InitializeComponent();
            // Инициализируем репозитории для работы с зданиями и атрибутами
            _buildingRepo = new BuildingRepository(_connectionString);
            _attributeRepo = new BuildingAttributeRepository(_connectionString);
            LoadBuildings();
        }

        private void LoadBuildings()
        {
            _buildings = _buildingRepo.Search("");
            
            // Заполняем ComboBox информацией о зданиях, например: "ID 3: ул. Ленина, д. 5"
            foreach (var b in _buildings)
            {
                BuildingComboBox.Items.Add($"ID {b.Id}: {b.Address}");
            }
        }
        

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрано ли здание из списка
            if (BuildingComboBox.SelectedIndex < 0)
            {
                StatusText.Text = "Выберите здание для добавления атрибута.";
                return;
            }

            // Получаем идентификатор выбранного здания
            int buildingId = _buildings[BuildingComboBox.SelectedIndex].Id;
            string section = SectionBox.Text;
            string key = KeyBox.Text;
            string value = ValueBox.Text;

            // Создаем объект атрибута
            var attr = new BuildingAttribute
            {
                BuildingId = buildingId,
                Section = section,
                Key = key,
                Value = value
            };

            try
            {
                _attributeRepo.AddAttribute(attr);
                StatusText.Text = "Атрибут успешно добавлен!";
                
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка при добавлении: " + ex.Message;
            }
        }
    }
}
