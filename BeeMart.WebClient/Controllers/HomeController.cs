using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BeeMart.WebClient.Models;

namespace BeeMart.WebClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Entry point: go to the POS if logged in, otherwise to login.
        return HttpContext.Session.GetString("auth.token") is { Length: > 0 }
            ? RedirectToAction("Index", "Transaction")
            : RedirectToAction("Login", "Auth");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
