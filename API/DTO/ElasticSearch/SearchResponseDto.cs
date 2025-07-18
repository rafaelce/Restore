namespace API.DTO.ElasticSearch;

public class SearchResponseDto
{
    public List<ProductSearchDto>? Products { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
