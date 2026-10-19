using System.Text.Json;
using BeeMart.WebClient.Models.Transactions;

namespace BeeMart.WebClient.Services;

/// <summary>
/// Typed access to WebClient session state: auth token, logged-in user, and cart.
/// Keeps the WebClient's state management (spec req 3) in one place.
/// </summary>
public static class SessionKeys
{
    public const string Token = "auth.token";
    public const string IdUser = "auth.idUser";
    public const string Username = "auth.username";
    public const string FullName = "auth.fullName";
    public const string Role = "auth.role";
    public const string Cart = "tx.cart";
}

public static class SessionExtensions
{
    public static void SetAuth(this ISession s, string token, string idUser, string username, string fullName, string role)
    {
        s.SetString(SessionKeys.Token, token);
        s.SetString(SessionKeys.IdUser, idUser);
        s.SetString(SessionKeys.Username, username);
        s.SetString(SessionKeys.FullName, fullName);
        s.SetString(SessionKeys.Role, role);
    }

    public static string? GetToken(this ISession s) => s.GetString(SessionKeys.Token);
    public static string? GetRole(this ISession s) => s.GetString(SessionKeys.Role);
    public static string? GetUsername(this ISession s) => s.GetString(SessionKeys.Username);
    public static bool IsLoggedIn(this ISession s) => !string.IsNullOrEmpty(s.GetToken());

    public static List<CartItem> GetCart(this ISession s)
    {
        var json = s.GetString(SessionKeys.Cart);
        return string.IsNullOrEmpty(json)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public static void SetCart(this ISession s, List<CartItem> cart) =>
        s.SetString(SessionKeys.Cart, JsonSerializer.Serialize(cart));

    public static void ClearCart(this ISession s) => s.Remove(SessionKeys.Cart);
}
