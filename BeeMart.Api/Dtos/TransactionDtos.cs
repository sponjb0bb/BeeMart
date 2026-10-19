using System.ComponentModel.DataAnnotations;

namespace BeeMart.Api.Dtos;

public class CreateTransactionLineDto
{
    [Required] public string IdProduct { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int Qty { get; set; }
}

public class CreateTransactionDto
{
    [Range(1, int.MaxValue)] public int IdPaymentMethod { get; set; }
    [MinLength(1)] public List<CreateTransactionLineDto> Items { get; set; } = new();
}

public class TransactionResponseDto
{
    public string IdTransaction { get; set; } = string.Empty;
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public decimal TotalPrice { get; set; }
}

public class TransactionHistoryDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Cashier { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
