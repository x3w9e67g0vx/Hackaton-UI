using System.Collections.ObjectModel;
using System.Windows.Input;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels
{
    public class ObjectsViewModel : ViewModelBase
    {
        private readonly string _connectionString;
        private readonly MainWindowViewModel _mainViewModel;
        private readonly BuildingRepository _buildingRepo;
        private readonly BuildingAttributeRepository _attributeRepo;

        public ObservableCollection<Building> Buildings { get; } = new();
        public ObservableCollection<BuildingAttribute> CurrentBuildingAttributes { get; } = new();

        private Building? _selectedBuilding;
        public Building? SelectedBuilding
        {
            get => _selectedBuilding;
            set
            {
                if (SetProperty(ref _selectedBuilding, value))
                {
                    if (_selectedBuilding != null)
                    {
                        var attrs = _attributeRepo.GetByBuildingId(_selectedBuilding.Id);
                        CurrentBuildingAttributes.Clear();
                        foreach (var attr in attrs)
                        {
                            CurrentBuildingAttributes.Add(attr);
                        }
                    }
                    else
                    {
                        CurrentBuildingAttributes.Clear();
                    }
                }
            }
        }

        private string _searchTerm = string.Empty;
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        // Новое свойство для хранения роли (например, "Admin", "Expert" или "Viewer")
        private string _role;
        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        // Команды выполняются всегда, а проверка на роль будет происходить в XAML (через привязку и конвертер).
        public ICommand AddBuildingCommand { get; }
        public ICommand EditBuildingCommand { get; }
        public ICommand DeleteBuildingCommand { get; }
        public ICommand SearchCommand { get; }

        public ObjectsViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _connectionString = "Data Source=hackaton.db;Version=3;";
            _buildingRepo = new BuildingRepository(_connectionString);
            _attributeRepo = new BuildingAttributeRepository(_connectionString);

            // Команды без встроенной проверки роли
            AddBuildingCommand = new RelayCommand(param => AddBuilding());
            EditBuildingCommand = new RelayCommand(param => EditBuilding());
            DeleteBuildingCommand = new RelayCommand(param => DeleteBuilding());
            SearchCommand = new RelayCommand(param => SearchBuildings());

            LoadBuildings();
        }

        private void LoadBuildings()
        {
            Buildings.Clear();
            // Используем метод поиска с пустым ключевым словом для получения всех объектов
            var list = _buildingRepo.Search("");
            foreach (var b in list)
                Buildings.Add(b);
        }
        public bool IsUserAllowed =>
            _mainViewModel.CurrentUser != null &&
            (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
             _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));

        private void AddBuilding()
        {
            var building = new Building { Address = "Новый объект" };
            _buildingRepo.Add(building);
            Buildings.Add(building);
        }

        private void EditBuilding()
        {
            if (SelectedBuilding == null) return;
            _buildingRepo.Update(SelectedBuilding);
        }

        private void DeleteBuilding()
        {
            if (SelectedBuilding == null) return;
            _buildingRepo.Delete(SelectedBuilding.Id);
            Buildings.Remove(SelectedBuilding);
        }

        private void SearchBuildings()
        {
            var result = _buildingRepo.Search(SearchTerm);
            Buildings.Clear();
            foreach (var b in result)
                Buildings.Add(b);
        }
    }
}
