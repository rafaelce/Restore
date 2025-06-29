using API.Entities;
using API.Extensions;
using API.Infra.EntityFramework;
using API.RequestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly ApplicationContext _context;

    public ProductsController(ApplicationContext context)
    => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductParams productParams)
    {
        var query =  _context.Products
            .Sort(productParams.OrderBy)
            .Search(productParams.SearchTerm)
            .Filter(productParams.Brands, productParams.Types)
            .AsQueryable();
               
       var products = await PagedList<Product>.ToPagedList(query, 
           productParams.PageNumber, productParams.PageSize);

       Response.AddPaginationHeader(products.Metadata);
           
       return products;
    }          

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        return product;
    }
    
    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        var brands = await _context.Products.Select(x => x.Brand).Distinct().ToListAsync();
        var types = await _context.Products.Select(x => x.Type).Distinct().ToListAsync();

        return Ok(new { brands, types });
    }
}
