namespace API.DTO.MassTransit;

// Mensagem para atualizar estoque
public class StockUpdateMessage
{
    public int ProductId { get; set; }
    public int QuantitySold { get; set; }
    public DateTime UpdatedAt { get; set; }
}
