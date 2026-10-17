namespace BeeMart.WebClient.Models.Catalog;

/// <summary>
/// Product as returned by the API search endpoint.
/// Columns map to the search table: Nama | Berat | Stock | Harga | Diskon.
/// </summary>
public class ProductDto
{
    public string IdProduct { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }

    /// <summary>Discount as a percentage, e.g. 5 means 5%.</summary>
    public decimal Discount { get; set; }
}
