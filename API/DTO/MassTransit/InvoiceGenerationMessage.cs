namespace API.DTO.MassTransit;

// Mensagem para gerar fatura
public class InvoiceGenerationMessage
{
    public int OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
}
