namespace BeeMart.Api.Dtos;

public class ProductDto
{
    public string IdProduct { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}

public class PaymentMethodDto
{
    public int IdPaymentMethod { get; set; }
    public string MethodName { get; set; } = string.Empty;
}
