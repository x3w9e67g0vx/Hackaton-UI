using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.Models;
using HackatonUi.Repositories;
using System;

namespace HackatonUi.Views;

    public partial class AddTaskDecisionWindow : Window
    {
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly TaskDecisionRepository _decisionRepo;
        private int _taskId;
        
        public AddTaskDecisionWindow(int taskId)
        {
            InitializeComponent();
            _taskId = taskId;
            _decisionRepo = new TaskDecisionRepository(_connectionString);
            TaskIdBox.Text = _taskId.ToString();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(StatusIdBox.Text, out int statusId))
            {
                StatusText.Text = "Введите корректный ID статуса.";
                return;
            }
            
            var decision = new TaskDecision
            {
                TaskId = _taskId,
                Description = DescriptionBox.Text,
                StatusId = statusId
            };
            
            try
            {
                _decisionRepo.AddDecision(decision);
                StatusText.Text = "Решение успешно добавлено.";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Ошибка: " + ex.Message;
            }
        }
    }
