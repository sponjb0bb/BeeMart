using BeeMart.WebClient.Models.Transactions;
using BeeMart.WebClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeeMart.WebClient.Controllers;

public class TransactionController : Controller
{
    private readonly BeeMartApiClient _api;
    public TransactionController(BeeMartApiClient api) => _api = api;

    // Guard: must be logged in.
    private IActionResult? RequireLogin() =>
        HttpContext.Session.IsLoggedIn() ? null : RedirectToAction("Login", "Auth");

    private async Task<NewTransactionViewModel> BuildViewModelAsync()
    {
        var pmResult = await _api.GetPaymentMethodsAsync();
        return new NewTransactionViewModel
        {
            Username = HttpContext.Session.GetUsername() ?? string.Empty,
            InvoiceNo = BuildPreviewInvoice(),
            Cart = HttpContext.Session.GetCart(),
            PaymentMethods = pmResult.Data ?? new(),
        };
    }

    /// <summary>
    /// Display-only invoice preview: [5-digit userID][ddmmyy][001].
    /// The API assigns the authoritative running number on submit.
    /// </summary>
    private string BuildPreviewInvoice()
    {
        var id = HttpContext.Session.GetString(SessionKeys.IdUser) ?? "00000";
        return $"{id}{DateTime.Now:ddMMyy}001";
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (RequireLogin() is { } r) return r;
        return View(await BuildViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(string keyword)
    {
        if (RequireLogin() is { } r) return r;

        var vm = await BuildViewModelAsync();
        vm.SearchKeyword = keyword;
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var res = await _api.SearchProductsAsync(keyword);
            if (res.Success) vm.SearchResults = res.Data ?? new();
            else vm.ErrorMessage = res.Error;
        }
        return View(nameof(Index), vm);
    }

    /// <summary>"Pilih" — re-run the search and surface the chosen product for the qty dialog.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(string idProduct, string keyword)
    {
        if (RequireLogin() is { } r) return r;

        var vm = await BuildViewModelAsync();
        vm.SearchKeyword = keyword;
        var res = await _api.SearchProductsAsync(keyword ?? string.Empty);
        if (res.Success)
        {
            vm.SearchResults = res.Data ?? new();
            vm.SelectedProduct = vm.SearchResults.FirstOrDefault(p => p.IdProduct == idProduct);
        }
        return View(nameof(Index), vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(string idProduct, string name, decimal price, decimal discount, int stock, int qty, string keyword)
    {
        if (RequireLogin() is { } r) return r;

        var vm = await BuildViewModelAsync();
        vm.SearchKeyword = keyword;

        if (qty < 1)
        {
            vm.ErrorMessage = "Qty minimal 1.";
        }
        else if (qty > stock)
        {
            vm.ErrorMessage = "Jumlah barang yang dimasukkan melebihi sisa stok.";
        }
        else
        {
            var cart = HttpContext.Session.GetCart();
            var existing = cart.FirstOrDefault(c => c.IdProduct == idProduct);
            if (existing is not null)
            {
                // Combined qty still can't exceed stock.
                if (existing.Qty + qty > stock)
                    vm.ErrorMessage = "Jumlah barang yang dimasukkan melebihi sisa stok.";
                else
                    existing.Qty += qty;
            }
            else
            {
                cart.Add(new CartItem { IdProduct = idProduct, Name = name, Price = price, Discount = discount, Qty = qty });
            }

            if (vm.ErrorMessage is null)
            {
                HttpContext.Session.SetCart(cart);
                vm.Cart = cart;
            }
        }
        return View(nameof(Index), vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(string idProduct)
    {
        if (RequireLogin() is { } r) return r;

        var cart = HttpContext.Session.GetCart();
        cart.RemoveAll(c => c.IdProduct == idProduct);
        HttpContext.Session.SetCart(cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int idPaymentMethod)
    {
        if (RequireLogin() is { } r) return r;

        var cart = HttpContext.Session.GetCart();
        if (cart.Count == 0)
        {
            var emptyVm = await BuildViewModelAsync();
            emptyVm.ErrorMessage = "Keranjang tidak boleh kosong.";
            return View(nameof(Index), emptyVm);
        }

        var request = new CreateTransactionRequest
        {
            IdPaymentMethod = idPaymentMethod,
            Items = cart.Select(c => new CreateTransactionLine { IdProduct = c.IdProduct, Qty = c.Qty }).ToList(),
        };

        var result = await _api.CreateTransactionAsync(request);

        var vm = await BuildViewModelAsync(); // fresh payment methods
        if (result.Success && result.Data is not null)
        {
            // Reset to initial state; show the authoritative invoice from the API.
            HttpContext.Session.ClearCart();
            vm.Cart = new();
            vm.InvoiceNo = result.Data.InvoiceNo;
            vm.SuccessMessage = $"Transaksi berhasil. Invoice: {result.Data.InvoiceNo}";
        }
        else
        {
            vm.ErrorMessage = result.Error ?? "Submit transaksi gagal.";
        }
        return View(nameof(Index), vm);
    }
}
