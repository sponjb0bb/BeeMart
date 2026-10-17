namespace BeeMart.Api.Entities;

/// <summary>Transaction line item (TrTransactionDetail).</summary>
public class TrTransactionDetail : AuditableEntity
{
    public string IdTransactionDetail { get; set; } = string.Empty; // NVARCHAR(36) PK
    public string IdTransaction { get; set; } = string.Empty;       // NVARCHAR(36) FK -> TrTransaction
    public string IdProduct { get; set; } = string.Empty;           // NVARCHAR(36) FK -> MsProduct
    public int Qty { get; set; }                                    // INT
    public decimal Price { get; set; }                              // DECIMAL(18,2) unit price at sale time
    public decimal Discount { get; set; }                           // DECIMAL(5,2) percentage
    public decimal SubTotal { get; set; }                           // DECIMAL(18,2)

    public TrTransaction? Transaction { get; set; }
    public MsProduct? Product { get; set; }
}
