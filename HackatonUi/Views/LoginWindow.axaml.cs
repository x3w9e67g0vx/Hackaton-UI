using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Data.SQLite;
using Avalonia.Rendering;
using HackatonUi.Data;
using HackatonUi.Models;
using HackatonUi.Repositories;
using HackatonUi.ViewModels;

namespace HackatonUi.Views;

public partial class LoginWindow : Window
{
    private readonly MainWindowViewModel _mainVM;

    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new LoginViewModel { CurrentWindow = this };
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        var name = UsernameBox.Text;
        var password = PasswordBox.Text;

        var mainVM = new MainWindowViewModel(); // <--- создаём экземпляр
    
        bool logged = mainVM.Login(name, password);
    
        if (logged)
        {
            ErrorText.Text = $"✅ Вход выполнен, роль: {mainVM.CurrentUser?.RoleName}";
            System.Threading.Thread.Sleep(100);
        
            var mainWindow = new MainWindow
            {
                DataContext = mainVM // <-- передаём ViewModel
            };
        
            mainWindow.Show();
            Close();
        }
        else
        {
            ErrorText.Text = "❌ Неверные данные";
        }
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        new RegisterWindow().Show();
        Close();
    }
}