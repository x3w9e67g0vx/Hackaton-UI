namespace HackatonUi.Models;

public class UserCredentials
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RoleId { get; set; }
    public string? RoleName { get; set; }  // заполняется при запросах
}