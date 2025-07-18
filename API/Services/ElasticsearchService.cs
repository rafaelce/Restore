using API.DTO.ElasticSearch;
using API.Services.Interfaces;
using Nest;

namespace API.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string IndexName = "products";

    public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    // Adiciona/atualiza um produto no índice
    public async Task<bool> IndexProductAsync(ProductSearchDto product)
    {
        try
        {
            var response = await _elasticClient.IndexAsync(product, i => i.Index(IndexName));
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar produto {ProductId}", product.Id);
            return false;
        }
    }

    // Atualiza um produto existente
    public async Task<bool> UpdateProductAsync(ProductSearchDto product)
    {
        try
        {
            var response = await _elasticClient.UpdateAsync<ProductSearchDto>(
                product.Id,
                u => u.Index(IndexName).Doc(product)
            );
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProductId}", product.Id);
            return false;
        }
    }

    // Remove um produto do índice
    public async Task<bool> DeleteProductAsync(int productId)
    {
        try
        {
            var response = await _elasticClient.DeleteAsync<ProductSearchDto>(
                productId,
                d => d.Index(IndexName)
            );
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto {ProductId}", productId);
            return false;
        }
    }

    // Verifica se o Elasticsearch está funcionando
    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _elasticClient.PingAsync();
            return response.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no health check do Elasticsearch");
            return false;
        }
    }

    // Realiza buscas com filtros
    public async Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request)
    {
        try
        {
            var searchDescriptor = new SearchDescriptor<ProductSearchDto>()
                .Index(IndexName)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .MultiMatch(mm => mm
                                .Fields(f => f
                                    .Field(ff => ff.Name, 2.0f)
                                    .Field(ff => ff.Description, 1.0f)
                                    .Field(ff => ff.Brand, 1.5f)
                                    .Field(ff => ff.Type, 1.5f)
                                )
                                .Query(request.Query)
                                .Type(TextQueryType.BestFields)
                                .Fuzziness(Fuzziness.Auto)
                            )
                        )
                        .Filter(f => f
                            .Range(r => r
                                .Field(ff => ff.Price)
                                .GreaterThanOrEquals((double?)(request.MinPrice ?? 0))
                                .LessThanOrEquals((double?)(request.MaxPrice ?? decimal.MaxValue))
                            )
                        )
                    )
                )
                .Sort(s => s
                    .Field(f => f
                        .Field(GetSortField(request.SortBy))
                        .Order(request.SortOrder.ToLower() == "desc" ? SortOrder.Descending : SortOrder.Ascending)
                    )
                )
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize);

            // Adicionar filtros adicionais se fornecidos
            if (!string.IsNullOrEmpty(request.Brand))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t.Field(ff => ff.Brand).Value(request.Brand))
                        )
                    )
                );
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                searchDescriptor = searchDescriptor.Query(q => q
                    .Bool(b => b
                        .Filter(f => f
                            .Term(t => t.Field(ff => ff.Type).Value(request.Type))
                        )
                    )
                );
            }

            var response = await _elasticClient.SearchAsync<ProductSearchDto>(searchDescriptor);

            if (!response.IsValid)
            {
                _logger.LogError("Erro na busca: {Error}", response.OriginalException?.Message);
                return new SearchResponseDto { Products = new List<ProductSearchDto>() };
            }

            var totalCount = (int)response.Total;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new SearchResponseDto
            {
                Products = response.Documents.ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
            return new SearchResponseDto { Products = new List<ProductSearchDto>() };
        }
    }

    // Recria todo o índice (útil para migrações)
    public async Task<bool> ReindexAllProductsAsync()
    {
        try
        {
            // Verificar se o índice existe
            var indexExists = await _elasticClient.Indices.ExistsAsync(IndexName);
            
            if (indexExists.Exists)
            {
                // Deletar índice existente
                await _elasticClient.Indices.DeleteAsync(IndexName);
            }

            // Criar novo índice com mapping correto
            var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                .Map<ProductSearchDto>(m => m
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Name)
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                            .Fielddata(true)
                        )
                        .Text(t => t.Name(n => n.Description)
                            .Fields(f => f
                                .Keyword(k => k.Name("keyword"))
                            )
                        )
                        .Keyword(t => t.Name(n => n.Brand))
                        .Keyword(t => t.Name(n => n.Type))
                        .Number(t => t.Name(n => n.Price).Type(NumberType.Double))
                        .Number(t => t.Name(n => n.QuantityInStock).Type(NumberType.Integer))
                        .Date(t => t.Name(n => n.CreatedAt))
                        .Date(t => t.Name(n => n.UpdatedAt))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                _logger.LogError("Erro ao criar índice: {Error}", createIndexResponse.OriginalException?.Message);
                return false;
            }

            _logger.LogInformation("Índice {IndexName} criado com sucesso", IndexName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reindexar produtos");
            return false;
        }
    }

    private string GetSortField(string sortBy)
    {
        return sortBy.ToLower() switch
        {
            "price" => "price",
            "brand" => "brand",
            "type" => "type",
            "createdat" => "createdAt",
            "updatedat" => "updatedAt",
            _ => "name.keyword"  // Mudou de "name" para "name.keyword"
        };
    }
}
