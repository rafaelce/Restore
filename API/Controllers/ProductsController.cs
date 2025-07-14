using API.DTO;
using API.Entities;
using API.Extensions;
using API.Infra.EntityFramework;
using API.RequestHelpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly ImageService _imageService;

    public ProductsController(ApplicationContext context, IMapper mapper, ImageService imageService){
        _imageService = imageService;
        _context = context;
        _mapper = mapper;
    }

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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(CreateProductDto productDto)
    {
        
        var product = _mapper.Map<Product>(productDto);


        if (productDto.File != null)
        {
            var imageResult = await _imageService.AddImageAsync(productDto.File);

            if (imageResult.Error != null)
            {
                return BadRequest(imageResult.Error.Message);
            }

            product.PictureUrl = imageResult.SecureUrl.AbsoluteUri;
            product.PublicId = imageResult.PublicId;
        }

         _context.Products.Add(product);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) 
            return CreatedAtAction(nameof(GetProduct), new { Id = product.Id }, product);

        return BadRequest("Problem creating new product");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult> UpdateProduct(UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FindAsync(updateProductDto.Id);

        if (product == null) return NotFound();

        _mapper.Map(updateProductDto, product);

        if (updateProductDto.File != null)
        {
            var imageResult = await _imageService.AddImageAsync(updateProductDto.File);

            if (imageResult.Error != null)
                return BadRequest(imageResult.Error.Message);

            if (!string.IsNullOrEmpty(product.PublicId))
                await _imageService.DeleteImageAsync(product.PublicId);

            product.PictureUrl = imageResult.SecureUrl.AbsoluteUri;
            product.PublicId = imageResult.PublicId;
        }

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return NoContent();

        return BadRequest("Problem updating product");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);

        if (!string.IsNullOrEmpty(product.PublicId))
            await _imageService.DeleteImageAsync(product.PublicId);

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Problem deleting product");
    }
}
