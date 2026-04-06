using HackatonUi.ViewModels;

namespace HackatonUi.Models
{
    public class DecisionAssignmentInfo 
    {
        public int Id { get; set; }
        public int DecisionId { get; set; }
        public int UserId { get; set; }
        public string AssignedAt { get; set; } = "";
        public string DecisionDescription { get; set; } = "";
        public string TaskDescription { get; set; } = ""; 
        public string Username { get; set; } = "";

        // Объединённая строка: Название задания - Название задачи
        public string DisplayText => $"{DecisionDescription} - {TaskDescription}";
        private readonly MainWindowViewModel _mainViewModel;
        public bool IsUserAllowed =>
            _mainViewModel.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));
    }
}