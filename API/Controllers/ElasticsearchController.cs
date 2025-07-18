using API.DTO.ElasticSearch;
using API.Extensions;
using API.Infra.EntityFramework;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ElasticsearchController : BaseApiController
{
    private readonly ApplicationContext _context;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<ElasticsearchController> _logger;

    public ElasticsearchController(
        ApplicationContext context,
        IElasticsearchService elasticsearchService,
        ILogger<ElasticsearchController> logger)
    {
        _context = context;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<ActionResult<bool>> HealthCheck()
    {
        var isHealthy = await _elasticsearchService.HealthCheckAsync();
        return Ok(isHealthy);
    }

    [HttpPost("search")]
    public async Task<ActionResult<SearchResponseDto>> SearchProducts([FromBody] SearchRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new { error = "Query é obrigatória" });
        }

        var result = await _elasticsearchService.SearchProductsAsync(request);
        return Ok(result);
    }

    [HttpPut("update-product")]
    public async Task<ActionResult<bool>> UpdateProduct([FromBody] ProductSearchDto product)
    {
        var result = await _elasticsearchService.UpdateProductAsync(product);
        return Ok(result);
    }

    [HttpPost("reindex")]
    public async Task<ActionResult<bool>> ReindexAllProducts()
    {
        var result = await _elasticsearchService.ReindexAllProductsAsync();
        return Ok(result);
    }

    // Busca todos os produtos do banco e indexa no Elasticsearch
    [HttpPost("index-all-products")]
    public async Task<ActionResult<string>> IndexAllProducts()
    {
        try
        {
            // Primeiro, recriar o índice
            var reindexResult = await _elasticsearchService.ReindexAllProductsAsync();
            if (!reindexResult)
            {
                return BadRequest("Erro ao recriar índice");
            }

            // Buscar todos os produtos do banco de dados
            var products = await _context.Products
                .Select(p => p.ToElasticsearchDto())
                .ToListAsync();

            if (!products.Any())
            {
                return Ok("Nenhum produto encontrado no banco de dados");
            }

            // Indexar cada produto
            int successCount = 0;
            int errorCount = 0;

            foreach (var product in products)
            {
                try
                {
                    var result = await _elasticsearchService.IndexProductAsync(product);
                    if (result) 
                    {
                        successCount++;
                    }
                    else
                    {
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao indexar produto {ProductId}", product.Id);
                    errorCount++;
                }
            }

            var message = $"Indexação concluída: {successCount} produtos indexados com sucesso, {errorCount} erros";
            _logger.LogInformation(message);
            
            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao indexar produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Sincroniza um produto específico com o Elasticsearch
    [HttpPost("sync-product/{id}")]
    public async Task<ActionResult<string>> SyncProduct(int id)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound($"Produto com ID {id} não encontrado");
            }

            var productDto = product.ToElasticsearchDto();
            var result = await _elasticsearchService.IndexProductAsync(productDto);

            if (result)
            {
                return Ok($"Produto {product.Name} sincronizado com sucesso");
            }
            else
            {
                return BadRequest($"Erro ao sincronizar produto {product.Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Remove um produto específico do Elasticsearch
    [HttpDelete("remove-product/{id}")]
    public async Task<ActionResult<string>> RemoveProduct(int id)
    {
        try
        {
            var result = await _elasticsearchService.DeleteProductAsync(id);

            if (result)
            {
                return Ok($"Produto com ID {id} removido do índice");
            }
            else
            {
                return BadRequest($"Erro ao remover produto com ID {id} do índice");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto {ProductId} do índice", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}
