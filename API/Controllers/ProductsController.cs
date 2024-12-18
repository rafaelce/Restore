using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        => _context = context;
        
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        => Ok( await _context.Products.ToListAsync());
        

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        => Ok(await _context.Products.FindAsync(id));
    }
}