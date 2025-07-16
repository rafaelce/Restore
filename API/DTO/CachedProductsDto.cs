using API.Entities;
using API.RequestHelpers;

namespace API.DTO;

public class CachedProductsDto
{
    public List<Product> Items { get; set; } = new();
    public PaginationMetadata Metadata { get; set; } = new();
    
    public static CachedProductsDto FromPagedList(PagedList<Product> pagedList)
    {
        return new CachedProductsDto
        {
            Items = pagedList.ToList(),
            Metadata = pagedList.Metadata
        };
    }
    
    public PagedList<Product> ToPagedList()
    {
        return new PagedList<Product>(Items, Metadata.TotalCount, Metadata.CurrentPage, Metadata.PageSize);
    }
} 