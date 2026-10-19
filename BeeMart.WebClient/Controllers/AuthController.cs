using BeeMart.WebClient.Models.Auth;
using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.WebClient.Controllers;

public class AuthController : Controller
{
    private readonly BeeMartApiClient _api;
    public AuthController(BeeMartApiClient api) => _api = api;

    // ---- Login ----
    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Index", "Transaction");
        return View(new LoginRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.LoginAsync(model);
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Invalid login");
            return View(model);
        }

        var d = result.Data;
        HttpContext.Session.SetAuth(d.Token, d.IdUser, d.Username, d.FullName, d.Role);
        return RedirectToAction("Index", "Transaction");
    }

    // ---- Register ----
    [HttpGet]
    public IActionResult Register() => View(new RegisterRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _api.RegisterAsync(model);
        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registrasi gagal.");
            return View(model);
        }

        // Auto-login after successful registration.
        var d = result.Data;
        HttpContext.Session.SetAuth(d.Token, d.IdUser, d.Username, d.FullName, d.Role);
        return RedirectToAction("Index", "Transaction");
    }

    // ---- Logout ----
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
