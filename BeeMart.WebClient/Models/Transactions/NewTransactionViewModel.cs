using BeeMart.WebClient.Models.Catalog;
using BeeMart.WebClient.Models.Transactions;

namespace BeeMart.WebClient.Models.Transactions;

/// <summary>Everything the New Transaction page needs to render.</summary>
public class NewTransactionViewModel
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    public string? SearchKeyword { get; set; }
    public List<ProductDto> SearchResults { get; set; } = new();

    public List<CartItem> Cart { get; set; } = new();
    public List<PaymentMethodDto> PaymentMethods { get; set; } = new();
    public int SelectedPaymentMethod { get; set; }

    // The product selected via "Pilih" (shows the qty dialog), if any.
    public ProductDto? SelectedProduct { get; set; }

    public decimal Total => Cart.Sum(c => c.SubTotal);
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
