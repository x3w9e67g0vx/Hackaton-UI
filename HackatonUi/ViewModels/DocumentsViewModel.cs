using System.Collections.ObjectModel;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels
{
    public class DocumentsViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly string _connectionString;
        private readonly BuildingRepository _buildingRepo;
        private readonly BuildingDocumentRepository _docRepo;
        
        public ObservableCollection<Building> Buildings { get; } = new ObservableCollection<Building>();

        public bool IsUserAllowed =>
            _mainViewModel?.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));

        public DocumentsViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _connectionString = "Data Source=hackaton.db;Version=3;";
            _buildingRepo = new BuildingRepository(_connectionString);
            _docRepo = new BuildingDocumentRepository(_connectionString);
            LoadBuildings();
        }
        
        public void LoadBuildings()
        {
            Buildings.Clear();
            var buildingList = _buildingRepo.Search("");
            foreach (var b in buildingList)
            {
                b.Documents = _docRepo.GetDocuments(b.Id);
                Buildings.Add(b);
            }
        }
    }
}