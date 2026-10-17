namespace BeeMart.WebClient.Models.Transactions;

/// <summary>
/// A line item held in WebClient cart state (Daftar Belanjaan).
/// Purely UI-side; the API recomputes authoritative prices on submit.
/// </summary>
public class CartItem
{
    public string IdProduct { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    /// <summary>Discount percentage applied to this product, e.g. 5 = 5%.</summary>
    public decimal Discount { get; set; }
    public int Qty { get; set; }

    /// <summary>Price * Qty after discount.</summary>
    public decimal SubTotal => Math.Round(Price * Qty * (1 - Discount / 100m), 2);
}
