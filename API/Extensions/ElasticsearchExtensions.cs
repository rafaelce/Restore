using API.DTO.ElasticSearch;
using API.Entities;

namespace API.Extensions;

public static class ElasticsearchExtensions
{
    public static ProductSearchDto ToElasticsearchDto(this Product product)
    {
        return new ProductSearchDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Brand = product.Brand,
            Type = product.Type,
            QuantityInStock = product.QuantityInStock,
            PictureUrl = product.PictureUrl
        };
    }
}
