using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HackatonUi.Data;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string _connectionString;
        private ViewModelBase? _currentView;
        private double _sidebarWidth = 70;
        private bool _isSidebarExpanded = true;

        public bool IsSidebarExpanded
        {
            get => _isSidebarExpanded;
            set
            {
                _isSidebarExpanded = value;
                OnPropertyChanged(nameof(IsSidebarExpanded));
                OnPropertyChanged(nameof(SidebarWidth));
            }
        }

        public ObservableCollection<Building> Buildings { get; } = new();
        
        private List<BuildingAttribute> _currentBuildingAttributes = new();
        public List<BuildingAttribute> CurrentBuildingAttributes
        {
            get => _currentBuildingAttributes;
            set => SetProperty(ref _currentBuildingAttributes, value);
        }
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
                        CurrentBuildingAttributes = AttributeRepo.GetByBuildingId(_selectedBuilding.Id);
                    }
                    else
                    {
                        CurrentBuildingAttributes = new List<BuildingAttribute>();
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
        
        public ICommand SearchCommand { get; }
        
        
        public bool IsUserAllowed =>
            CurrentUser != null &&
            (CurrentUser.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
             CurrentUser.RoleName.Equals("Expert", StringComparison.OrdinalIgnoreCase));


        public MainWindowViewModel()
        {
            _connectionString = "Data Source=hackaton.db;Version=3;";
            DatabaseInitializer.Initialize(_connectionString);

            CurrentView = new HomeViewModel(this);

            UserRepository = new UserRepository(_connectionString);
            BuildingRepo = new BuildingRepository(_connectionString);
            AttributeRepo = new BuildingAttributeRepository(_connectionString);
            
            
        }

        public UserRepository UserRepository { get; }
        public BuildingRepository BuildingRepo { get; }
        public BuildingAttributeRepository AttributeRepo { get; }
       

        private UserCredentials? _currentUser;
        public UserCredentials? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUserLoggedIn));
            }
        }
    
        public bool IsUserLoggedIn => CurrentUser != null;

        public ViewModelBase? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public double SidebarWidth
        {
            get => _sidebarWidth;
            set
            {
                _sidebarWidth = value;
                OnPropertyChanged();
            }
        }
        public void Navigate(string viewTag)
        {
            switch (viewTag)
            {
                case "Home":
                    CurrentView = new HomeViewModel(this);
                    break;
                case "Objects":
                    CurrentView = new ObjectsViewModel(this);
                    break;
                case "Documents":
                    CurrentView = new DocumentsViewModel(this);
                    break;
                case "XmlImport":
                    CurrentView = new XmlImportViewModel(this);
                    break;
                case "Protocols":
                    CurrentView = new ProtocolsViewModel(this);
                    break;
                case "Control":
                    CurrentView = new ControlViewModel(this);
                    break;
                case "Users":
                    CurrentView = new UserViewModel(this);
                    break;
            }
        }

        public bool Login(string username, string password)
        {
            CurrentUser = UserRepository.Login(username, password);
            return CurrentUser != null;
        }

        public bool Register(string username, string password, string role)
        {
            return UserRepository.Register(username, password, role);
        }
        
        
    }
}
