namespace BeeMart.Api.Entities;

/// <summary>Payment method lookup (LtPaymentMethod): Cash, Debit, Credit.</summary>
public class LtPaymentMethod : AuditableEntity
{
    public int IdPaymentMethod { get; set; }                // INT PK
    public string MethodName { get; set; } = string.Empty;  // NVARCHAR(50)

    public ICollection<TrTransaction> Transactions { get; set; } = new List<TrTransaction>();
}
