namespace BeeMart.Api.Entities;

/// <summary>
/// Audit columns present on every table in the BeeMart ERD.
/// Stsrc is the soft-delete / active flag (BIT).
/// </summary>
public abstract class AuditableEntity
{
    public bool Stsrc { get; set; } = true;
    public DateTime Datein { get; set; }
    public string? Userin { get; set; }
    public DateTime? Dateup { get; set; }
    public string? Userup { get; set; }
}
