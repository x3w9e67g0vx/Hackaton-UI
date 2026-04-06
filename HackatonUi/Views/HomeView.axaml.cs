using Avalonia.Controls;
using Avalonia.Interactivity;
using HackatonUi.ViewModels;

namespace HackatonUi.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var window = this.VisualRoot as Window;
        if (window != null)
        {
            var loginWindow = new LoginWindow(); // ваша форма логина
            loginWindow.Show();
            window.Close();
        }
    }   

    private void Navigate_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && sender is Button button && button.Tag is string tag)
        {
            vm.Navigate(tag);
        }
    }
}