namespace API.DTO.ElasticSearch;

public class SearchRequestDto
{
    public required string Query { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "name";
    public string SortOrder { get; set; } = "asc";
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Brand { get; set; }
    public string? Type { get; set; }
}
