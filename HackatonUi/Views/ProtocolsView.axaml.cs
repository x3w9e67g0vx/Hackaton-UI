using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using HackatonUi.Models;
using HackatonUi.Repositories;
using HackatonUi.ViewModels;

namespace HackatonUi.Views
{
    public partial class ProtocolsView : UserControl
    {
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly InspectionTaskRepository _taskRepo;
        private readonly TaskDecisionRepository _decisionRepo;

        public ProtocolsView()
        {
            InitializeComponent();
            _taskRepo = new InspectionTaskRepository(_connectionString);
            _decisionRepo = new TaskDecisionRepository(_connectionString);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolsViewModel vm)
            {
                if (int.TryParse(SearchBox.Text, out int buildingId))
                    vm.LoadTasks(buildingId);
                else
                    vm.LoadTasks();
            }
        }

        private void OpenAddTask_Click(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddInspectionTaskWindow();
            addTaskWindow.Closed += (s, args) =>
            {
                if (DataContext is ProtocolsViewModel vm)
                    vm.LoadTasks();
            };
            addTaskWindow.Show();
        }

        private void OpenAddDecision_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is InspectionTask selectedTask)
            {
                var addDecisionWindow = new AddTaskDecisionWindow(selectedTask.Id);
                addDecisionWindow.Closed += (s, args) =>
                {
                    if (DataContext is ProtocolsViewModel vm)
                        vm.LoadTasks();
                };
                addDecisionWindow.Show();
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolsViewModel vm && vm.SelectedTask != null)
            {
                _taskRepo.DeleteTask(vm.SelectedTask.Id);
                vm.LoadTasks();
            }
            else
            {
                Console.WriteLine("Не выбрана задача для удаления.");
            }
        }

        private void DeleteDecisionButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolsViewModel vm && vm.SelectedTask != null)
            {
                if (vm.SelectedTask.SelectedDecision != null)
                {
                    _decisionRepo.DeleteDecision(vm.SelectedTask.SelectedDecision.Id);
                    vm.LoadTasks();
                }
                else
                {
                    Console.WriteLine("Не выбрано решение для удаления.");
                }
            }
        }
    }
}
