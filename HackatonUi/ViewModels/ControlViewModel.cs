using System.Collections.ObjectModel;
using System.Linq;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels
{
    public class ControlViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly InspectionTaskRepository _taskRepo;
        private readonly TaskDecisionRepository _decisionRepo;
        private readonly TaskStatusRepository _statusRepo;

        // Все задания из базы данных.
        public ObservableCollection<InspectionTask> AllTasks { get; } = new ObservableCollection<InspectionTask>();

        // Отфильтрованные задания для отображения.
        public ObservableCollection<InspectionTask> FilteredTasks { get; } = new ObservableCollection<InspectionTask>();

        // Список статусов из базы.
        public ObservableCollection<TaskStatus> Statuses { get; } = new ObservableCollection<TaskStatus>();

        // Выбранный объект статуса.
        private TaskStatus? _selectedStatusItem;
        public TaskStatus? SelectedStatusItem
        {
            get => _selectedStatusItem;
            set
            {
                if (SetProperty(ref _selectedStatusItem, value))
                {
                    int id = _selectedStatusItem?.Id ?? 0;
                    SelectedStatus = id;
                }
            }
        }
        public bool IsUserAllowed =>
            _mainViewModel.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));

        private int _selectedStatus;
        
        /// Если SelectedStatus == 0, показываются все задания, иначе фильтрация по заданному Id.
        public int SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                    FilterTasks();
            }
        }

        public ControlViewModel(MainWindowViewModel mainViewModel)
        {
            _taskRepo = new InspectionTaskRepository(_connectionString);
            _decisionRepo = new TaskDecisionRepository(_connectionString);
            _statusRepo = new TaskStatusRepository(_connectionString);
            _mainViewModel = mainViewModel;
            
            LoadStatuses();
            LoadTasks();

            
            SelectedStatus = 0;
        }

        public void LoadStatuses()
        {
            Statuses.Clear();
            // Добавляем опцию "Все"
            Statuses.Add(new TaskStatus { Id = 0, StatusName = "Все" });
            var statusesFromDb = _statusRepo.GetAllStatuses();
            foreach (var status in statusesFromDb)
            {
                Statuses.Add(status);
            }
        }

        public void LoadTasks()
        {
            AllTasks.Clear();
            var tasks = _taskRepo.GetAllTasks();
            foreach (var task in tasks)
            {
                task.Decisions = _decisionRepo.GetDecisionsByTaskId(task.Id);
                AllTasks.Add(task);
            }
            FilterTasks();
        }

        private void FilterTasks()
        {
            FilteredTasks.Clear();
            // Если у задания есть хотя бы одно решение, второе – берем статус первого, иначе считаем статус = 0.
            foreach (var task in AllTasks)
            {
                int taskStatus = task.Decisions.FirstOrDefault()?.StatusId ?? 0;
                if (SelectedStatus == 0 || taskStatus == SelectedStatus)
                {
                    FilteredTasks.Add(task);
                }
            }
        }
    }
}
