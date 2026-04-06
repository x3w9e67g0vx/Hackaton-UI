namespace HackatonUi.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    public HomeViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
}