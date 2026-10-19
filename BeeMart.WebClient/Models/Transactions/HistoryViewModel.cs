using BeeMart.WebClient.Models.Transactions;

namespace BeeMart.WebClient.Models.Transactions;

public class HistoryViewModel
{
    public DateTime? Date { get; set; }
    public int Page { get; set; } = 1;
    public PagedResult<TransactionHistoryItem>? Result { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public bool Searched { get; set; }
    public string? ErrorMessage { get; set; }
}
