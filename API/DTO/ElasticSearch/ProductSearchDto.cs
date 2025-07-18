namespace API.DTO.ElasticSearch;

public class ProductSearchDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Brand { get; set; }
    public string? Type { get; set; }
    public int QuantityInStock { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
