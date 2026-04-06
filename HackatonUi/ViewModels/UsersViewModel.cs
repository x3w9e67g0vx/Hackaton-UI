using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly string _connectionString = "Data Source=hackaton.db;Version=3;";
        private readonly TaskDecisionRepository _decisionRepo;
        private readonly UserRepository _userRepo;
        private readonly DecisionAssignmentRepository _assignmentRepo;
        
        public ObservableCollection<TaskDecision> Decisions { get; } = new ObservableCollection<TaskDecision>();
        public ObservableCollection<UserCredentials> Viewers { get; } = new ObservableCollection<UserCredentials>();
        public ObservableCollection<DecisionAssignmentInfo> Assignments { get; } = new ObservableCollection<DecisionAssignmentInfo>();
        public ObservableCollection<GroupedAssignment> GroupedAssignments { get; } = new ObservableCollection<GroupedAssignment>();

        private TaskDecision? _selectedDecision;
        public TaskDecision? SelectedDecision
        {
            get => _selectedDecision;
            set => SetProperty(ref _selectedDecision, value);
        }

        private UserCredentials? _selectedViewer;
        public UserCredentials? SelectedViewer
        {
            get => _selectedViewer;
            set => SetProperty(ref _selectedViewer, value);
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public bool IsUserAllowed =>
            _mainViewModel.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));

        // Команда для удаления назначения (оставляем, если используется)
        public ICommand DeleteAssignmentCommand { get; }

        public UserViewModel(MainWindowViewModel mainViewModel)
        {
            _decisionRepo = new TaskDecisionRepository(_connectionString);
            _userRepo = new UserRepository(_connectionString);
            _assignmentRepo = new DecisionAssignmentRepository(_connectionString);
            _mainViewModel = mainViewModel;

            DeleteAssignmentCommand = new RelayCommand(param =>
            {
                if (param is DecisionAssignmentInfo assignment)
                {
                    _assignmentRepo.DeleteAssignment(assignment.Id);
                    LoadAssignments();
                }
            });

            LoadDecisions();
            LoadViewers();
            LoadAssignments();
        }

        public void LoadDecisions()
        {
            Decisions.Clear();
            var decisions = _decisionRepo.GetAllDecisions();
            foreach (var d in decisions)
            {
                Decisions.Add(d);
            }
        }

        public void LoadViewers()
        {
            Viewers.Clear();
            var viewers = _userRepo.GetUsersByRole("viewer");
            foreach (var user in viewers)
            {
                Viewers.Add(user);
            }
        }

        public void LoadAssignments()
        {
            Assignments.Clear();
            var assignments = _assignmentRepo.GetAllAssignments();
            foreach (var a in assignments)
            {
                Assignments.Add(a);
            }
            GroupAssignments();
        }

        private void GroupAssignments()
        {
            GroupedAssignments.Clear();
            var groups = Assignments
                .GroupBy(a => a.DecisionDescription)
                .Select(g => new GroupedAssignment
                {
                    DecisionDescription = g.Key,
                    Assignments = new ObservableCollection<DecisionAssignmentInfo>(g)
                });
            foreach (var grp in groups)
            {
                GroupedAssignments.Add(grp);
            }
        }

        public async void Assign()
        {
            if (SelectedDecision != null && SelectedViewer != null)
            {
                bool added = _assignmentRepo.AssignUserToDecision(SelectedDecision.Id, SelectedViewer.Id);
                if (!added)
                {
                    // Если назначение уже существует, устанавливаем сообщение об ошибке.
                    ErrorMessage = "Этот пользователь уже назначен для данного решения.";
                    // Сообщение будет видно в интерфейсе, затем очистится через 3 секунды.
                    await Task.Delay(3000);
                    ErrorMessage = "";
                }
                LoadAssignments();
            }
        }
        public void DeleteAssignment(DecisionAssignmentInfo assignment)
        {
            _assignmentRepo.DeleteAssignment(assignment.Id);
            LoadAssignments();
        }
    }
}
