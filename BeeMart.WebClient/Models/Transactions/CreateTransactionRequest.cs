using System.ComponentModel.DataAnnotations;

namespace BeeMart.WebClient.Models.Transactions;

/// <summary>
/// Payload posted to the API to create a transaction.
/// The API owns invoice-number generation, pricing, and stock checks.
/// </summary>
public class CreateTransactionRequest
{
    [Required(ErrorMessage = "Metode Pembayaran wajib dipilih.")]
    public int IdPaymentMethod { get; set; }

    [MinLength(1, ErrorMessage = "Keranjang tidak boleh kosong.")]
    public List<CreateTransactionLine> Items { get; set; } = new();
}

/// <summary>One product line in a create-transaction request.</summary>
public class CreateTransactionLine
{
    [Required]
    public string IdProduct { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Qty minimal 1.")]
    public int Qty { get; set; }
}
