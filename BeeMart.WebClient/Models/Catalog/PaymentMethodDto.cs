namespace BeeMart.WebClient.Models.Catalog;

/// <summary>
/// Payment method option, fetched dynamically from the API (Cash, Debit, Credit).
/// </summary>
public class PaymentMethodDto
{
    public int IdPaymentMethod { get; set; }
    public string MethodName { get; set; } = string.Empty;
}
