using API.Entities;
using API.Infra.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ApplicationContext context) : ControllerBase
{
    [HttpGet]

    public async Task<ActionResult<List<Product>>> GetProducts(){
        return await context.Products.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id){

        var product = await context.Products.FindAsync(id);

        if(product == null) return NotFound();

        return product;
    }
}
