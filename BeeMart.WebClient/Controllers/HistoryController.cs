using BeeMart.WebClient.Models.Transactions;
using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.WebClient.Controllers;

public class HistoryController : Controller
{
    private readonly BeeMartApiClient _api;
    public HistoryController(BeeMartApiClient api) => _api = api;

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? date, int page = 1)
    {
        if (!HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Login", "Auth");

        var vm = new HistoryViewModel
        {
            Date = date,
            Page = page < 1 ? 1 : page,
            Role = HttpContext.Session.GetRole() ?? string.Empty,
            Username = HttpContext.Session.GetUsername() ?? string.Empty,
        };

        // Spec: user must pick a date before searching.
        if (date is null)
            return View(vm);

        vm.Searched = true;
        var res = await _api.GetHistoryAsync(date.Value, vm.Page);
        if (res.Success) vm.Result = res.Data;
        else vm.ErrorMessage = res.Error;

        return View(vm);
    }
}
