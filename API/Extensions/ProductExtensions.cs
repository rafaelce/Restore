using API.Entities;
using API.RequestHelpers;

namespace API.Extensions;

public static class ProductExtensions
{
    public static IQueryable<Product> Sort(this IQueryable<Product> query, string? orderBy)
    => orderBy switch
    {
        "price" => query.OrderBy(p => p.Price),
        "priceDesc" => query.OrderByDescending(p => p.Price),
        _ => query.OrderBy(p => p.Name),
    };

    public static IQueryable<Product> Search(this IQueryable<Product> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return query.Where(p => p.Name.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Product> Filter(this IQueryable<Product> query,
                                         string? brands,
                                         string? types)
    {
        var brandList = new List<string>();
        var typeList = new List<string>();

        if (!string.IsNullOrEmpty(brands))
        {
            brandList.AddRange([.. brands.ToLower().Split(",")]);
        }

        if (!string.IsNullOrEmpty(types))
        {
            typeList.AddRange([.. types.ToLower().Split(",")]);
        }

        query = query.Where(x => brandList.Count == 0 || brandList.Contains(x.Brand.ToLower()));
        query = query.Where(x => typeList.Count == 0 || typeList.Contains(x.Type.ToLower()));

        return query;
    }

    // Método auxiliar para gerar chave de cache legível
    public static string GenerateProductsCacheKey(ProductParams productParams)
    {
        var keyParts = new List<string>
        {
            "products",
            $"search:{productParams.SearchTerm ?? "all"}",
            $"order:{productParams.OrderBy ?? "name"}",
            $"page:{productParams.PageNumber}",
            $"size:{productParams.PageSize}"
        };

        // Adiciona filtros apenas se existirem
        if (productParams.Brands?.Any() == true)
        {
            keyParts.Add($"brands:{string.Join("-", productParams.Brands)}");
        }

        if (productParams.Types?.Any() == true)
        {
            keyParts.Add($"types:{string.Join("-", productParams.Types)}");
        }

        return string.Join(":", keyParts);
    }          

}