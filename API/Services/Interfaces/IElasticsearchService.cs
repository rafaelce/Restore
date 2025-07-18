using API.DTO.ElasticSearch;

namespace API.Services.Interfaces;

public interface IElasticsearchService
{
    Task<bool> IndexProductAsync(ProductSearchDto product);
    Task<bool> UpdateProductAsync(ProductSearchDto product);
    Task<bool> DeleteProductAsync(int productId);
    Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request);
    Task<bool> ReindexAllProductsAsync();
    Task<bool> HealthCheckAsync();
}
