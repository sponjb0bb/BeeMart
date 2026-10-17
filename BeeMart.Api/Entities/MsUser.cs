namespace BeeMart.Api.Entities;

/// <summary>User master (MsUser). IdUser is a 5-char string, used in invoice numbers.</summary>
public class MsUser : AuditableEntity
{
    public string IdUser { get; set; } = string.Empty;      // NVARCHAR(5) PK
    public string Username { get; set; } = string.Empty;    // NVARCHAR(50)
    public string FullName { get; set; } = string.Empty;    // NVARCHAR(300)
    public string PasswordHash { get; set; } = string.Empty; // NVARCHAR(255)
    public string Role { get; set; } = string.Empty;        // NVARCHAR(20) "Admin" | "Cashier"

    public ICollection<TrTransaction> Transactions { get; set; } = new List<TrTransaction>();
}
