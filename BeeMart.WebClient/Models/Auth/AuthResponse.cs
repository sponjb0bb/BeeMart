namespace BeeMart.WebClient.Models.Auth;

/// <summary>
/// Shape of the API's successful login response: identity + bearer token.
/// </summary>
public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string IdUser { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
