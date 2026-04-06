using Avalonia.Controls;
using HackatonUi.Models;
using HackatonUi.Repositories;

namespace HackatonUi.ViewModels;

public class LoginViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
    public Window? CurrentWindow { get; set; }
    public UserCredentials? LoggedInUser { get; private set; }
    
}