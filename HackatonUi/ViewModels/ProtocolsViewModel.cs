using System.Collections.ObjectModel;
using System.Windows.Input;
using HackatonUi.Models;
using HackatonUi.Repositories;
using HackatonUi.Views;

namespace HackatonUi.ViewModels
{
    public class ProtocolsViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly InspectionTaskRepository _taskRepo;
        private readonly TaskDecisionRepository _decisionRepo;

        public ObservableCollection<InspectionTask> Tasks { get; } = new ObservableCollection<InspectionTask>();
        public ObservableCollection<TaskDecision> AllDecisions { get; } = new ObservableCollection<TaskDecision>();

        private InspectionTask? _selectedTask;
        public InspectionTask? SelectedTask
        {
            get => _selectedTask;
            set
            {
                SetProperty(ref _selectedTask, value);
                // Загружаем решения для выбранной задачи
                SelectedDecision = null; // Сброс предыдущего выбора
            }
        }

        private TaskDecision? _selectedDecision;
        public TaskDecision? SelectedDecision
        {
            get => _selectedDecision;
            set => SetProperty(ref _selectedDecision, value);
        }

        public bool IsUserAllowed =>
            _mainViewModel.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));

        public ICommand EditTaskAndDecisionCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public ProtocolsViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _taskRepo = new InspectionTaskRepository(_connectionString);
            _decisionRepo = new TaskDecisionRepository(_connectionString);
            LoadTasks();

            EditTaskAndDecisionCommand = new RelayCommand(_ =>
            {
                if (IsUserAllowed)
                {
                    OpenEditTaskWindow();
                }
            });

            SaveChangesCommand = new RelayCommand(_ =>
            {
                if (SelectedTask != null)
                {
                    _taskRepo.UpdateTask(SelectedTask);
                }
                if (SelectedDecision != null)
                {
                    _decisionRepo.UpdateDecision(SelectedDecision);
                }
                LoadTasks();
            });
        }

        public void LoadTasks()
        {
            Tasks.Clear();
            var taskList = _taskRepo.GetAllTasks();
            foreach (var task in taskList)
            {
                task.Decisions = _decisionRepo.GetDecisionsByTaskId(task.Id);
                Tasks.Add(task);
            }
            UpdateAllDecisions();
        }
        public void LoadTasks(int buildingId)
        {
            Tasks.Clear();
            var taskList = _taskRepo.GetTasksByBuildingId(buildingId);
            foreach (var task in taskList)
            {
                task.Decisions = _decisionRepo.GetDecisionsByTaskId(task.Id);
                Tasks.Add(task);
            }
            UpdateAllDecisions();
        }


        public void UpdateAllDecisions()
        {
            AllDecisions.Clear();
            foreach (var task in Tasks)
            {
                foreach (var dec in task.Decisions)
                {
                    AllDecisions.Add(dec);
                }
            }
        }

        private void OpenEditTaskWindow()
        {
            var editWindow = new EditTaskAndDecisionWindow(this);
            editWindow.Closed += (s, e) => LoadTasks();
            editWindow.Show();
        }
    }
}
