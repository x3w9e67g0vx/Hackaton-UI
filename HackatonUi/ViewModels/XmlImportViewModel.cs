namespace HackatonUi.ViewModels;

public class XmlImportViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    
    public XmlImportViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
    public bool IsUserAllowed =>
        _mainViewModel.CurrentUser != null &&
        (_mainViewModel.CurrentUser.RoleName.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) ||
         _mainViewModel.CurrentUser.RoleName.Equals("Expert", System.StringComparison.OrdinalIgnoreCase));
}