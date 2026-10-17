namespace BeeMart.Api.Entities;

/// <summary>Product master (MsProduct).</summary>
public class MsProduct : AuditableEntity
{
    public string IdProduct { get; set; } = string.Empty;   // NVARCHAR(36) PK
    public string Name { get; set; } = string.Empty;        // NVARCHAR(100)
    public decimal Weight { get; set; }                     // DECIMAL(10,2)
    public int Stock { get; set; }                          // INT
    public decimal Price { get; set; }                      // DECIMAL(18,2)
    public decimal Discount { get; set; }                   // DECIMAL(5,2) percentage

    public ICollection<TrTransactionDetail> TransactionDetails { get; set; } = new List<TrTransactionDetail>();
}
