namespace BeeMart.WebClient.Models.Transactions;

/// <summary>
/// One row in the History Transaction table.
/// Columns: No. Transaksi | Tanggal & Jam | Kasir | Jml Item | Pembayaran | Total Harga.
/// </summary>
public class TransactionHistoryItem
{
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }

    /// <summary>Cashier full name (or username) who made the transaction.</summary>
    public string Cashier { get; set; } = string.Empty;

    /// <summary>Total number of items (sum of quantities) in the transaction.</summary>
    public int ItemCount { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
