namespace Main.Entities;
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
}

