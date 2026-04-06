using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Data.SQLite;
using HackatonUi.Data;
using HackatonUi.Repositories;
using HackatonUi.ViewModels;

namespace HackatonUi.Views;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
       
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    { 
        try
        {
            var mainVM = new MainWindowViewModel(); // <--- создаём экземпляр
            var name = UsernameBox.Text;
            var password = PasswordBox.Text;
            var role = RoleBox.Text;
            
            var success = mainVM.Register(name, password, role);
            
            if (success)
            {
                ErrorText.Text = "✅ Успешная регистрация";
                new LoginWindow().Show();
                Close(); 
            }
            else
            {
                ErrorText.Text = "⚠️ Пользователь уже существует";
            }
        }
        catch (SQLiteException ex)
        {
            ErrorText.Text = $"⛔ Ошибка: {ex.Message}";
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        Close();
    }
}