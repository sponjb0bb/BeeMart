namespace BeeMart.WebClient.Models.Transactions;

/// <summary>
/// API response after a successful transaction submit.
/// </summary>
public class TransactionResponse
{
    public string IdTransaction { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal TotalPrice { get; set; }
}
