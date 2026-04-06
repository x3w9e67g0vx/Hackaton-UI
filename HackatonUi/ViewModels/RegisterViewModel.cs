using System;
using System.Collections.Generic;
using Avalonia.Controls;
using HackatonUi.Repositories;
using HackatonUi.Views;

namespace HackatonUi.ViewModels;

public class RegisterViewModel 
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string SelectedRole { get; set; } = "Viewer";
    public string ErrorMessage { get; set; } = "";
    public List<string> Roles { get; } = UserRepository.GetAllRoleId();
    public Window? CurrentWindow { get; set; }
    
    
    
}