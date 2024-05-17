namespace ReferMe.Models;

public sealed class LoginResponse
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public string? Data { get; set; }
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
}