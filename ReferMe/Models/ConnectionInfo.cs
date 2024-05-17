namespace ReferMe.Models;

public sealed class ConnectionInfo
{
    public bool IsConnected { get; set; }
    public UserInfo? User { get; set; }
}

public sealed class UserInfo
{
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsEmailConfirmed { get; init; }
}