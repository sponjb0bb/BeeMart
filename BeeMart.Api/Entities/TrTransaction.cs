namespace BeeMart.Api.Entities;

/// <summary>Transaction header (TrTransaction).</summary>
public class TrTransaction : AuditableEntity
{
    public string IdTransaction { get; set; } = string.Empty; // NVARCHAR(36) PK
    public string InvoiceNo { get; set; } = string.Empty;     // NVARCHAR(20)
    public DateTime TransactionDate { get; set; }             // DATETIME2
    public string IdUser { get; set; } = string.Empty;        // NVARCHAR(5) FK -> MsUser
    public int IdPaymentMethod { get; set; }                  // INT FK -> LtPaymentMethod
    public decimal TotalPrice { get; set; }                   // DECIMAL(18,2)

    public MsUser? User { get; set; }
    public LtPaymentMethod? PaymentMethod { get; set; }
    public ICollection<TrTransactionDetail> Details { get; set; } = new List<TrTransactionDetail>();
}
