using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HackatonUi.ViewModels;

namespace HackatonUi.Views
{
    public partial class AddDocumentWindow : Window
    {
        private readonly BuildingRepository _buildingRepo;
        private readonly BuildingDocumentRepository _docRepo;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private List<Building> _buildings = new List<Building>();
       

        public AddDocumentWindow()
        {
            InitializeComponent();
            _buildingRepo = new BuildingRepository(_connectionString);
            _docRepo = new BuildingDocumentRepository(_connectionString);
            LoadBuildings();
        }
        private void LoadBuildings()
        {
            _buildings = _buildingRepo.Search("");
            foreach (var b in _buildings)
            {
                BuildingComboBox.Items.Add($"ID {b.Id}: {b.Address}");
            }
        }

        private async void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите файл",
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Все файлы", Extensions = { "*" } },
                    new FileDialogFilter { Name = "Txt файлы", Extensions = { "txt" } },
                    
                }
            };

            var result = await dialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                FilePathBox.Text = result[0];
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (BuildingComboBox.SelectedIndex < 0)
            {
                StatusText.Text = "Пожалуйста, выберите здание.";
                return;
            }

            int buildingId = _buildings[BuildingComboBox.SelectedIndex].Id;
            string filePath = FilePathBox.Text;
            string uploader = UploaderBox.Text;
            var uploadDate = UploadDatePicker.SelectedDate ?? DateTime.Now;

            var document = new BuildingDocument
            {
                BuildingId = buildingId,
                FilePath = filePath,
                UploadedBy = uploader,
                UploadedAt = uploadDate
            };

            try
            {
                _docRepo.AddDocument(document);
                StatusText.Text = "Документ успешно добавлен!";
                
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}
