using API.DTO;
using API.Entities;
using API.Extensions;
using API.Infra.EntityFramework;
using API.RequestHelpers;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class ProductsController(ApplicationContext _context, IMapper _mapper, ImageService _imageService, IRedisCacheService _cacheService) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductParams productParams)
    {
        // Gerando chave de cache simples e legível
        var cacheKey = ProductExtensions.GenerateProductsCacheKey(productParams);
        
        // Tentando buscar do cache primeiro
        var cachedProductsDto = await _cacheService.GetAsync<CachedProductsDto>(cacheKey);
        if (cachedProductsDto != null)
        {
            var cachedProducts = cachedProductsDto.ToPagedList();
            Response.AddPaginationHeader(cachedProducts.Metadata);
            return cachedProducts;
        }

        // Se não encontrou no cache, busca no banco
        var query =  _context.Products
            .Sort(productParams.OrderBy)
            .Search(productParams.SearchTerm)
            .Filter(productParams.Brands, productParams.Types)
            .AsQueryable();
               
       var products = await PagedList<Product>.ToPagedList(query, 
           productParams.PageNumber, productParams.PageSize);

       // Salva no cache por 10 minutos usando DTO
       var productsDto = CachedProductsDto.FromPagedList(products);
       await _cacheService.SetAsync(cacheKey, productsDto, TimeSpan.FromMinutes(10));

       Response.AddPaginationHeader(products.Metadata);
           
       return products;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // Cache para produto individual
        var cacheKey = $"product:single:{id}";
        var cachedProduct = await _cacheService.GetAsync<Product>(cacheKey);
        
        if (cachedProduct != null)
            return cachedProduct;

        var product = await _context.Products.FindAsync(id);

        if (product == null) return NotFound();

        // Salva no cache por 30 minutos
        await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));

        return product;
    }
    
    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        // Cache para filtros
        var cacheKey = "products:filters";
        var cachedFilters = await _cacheService.GetAsync<object>(cacheKey);
        
        if (cachedFilters != null)
            return Ok(cachedFilters);

        var brands = await _context.Products.Select(x => x.Brand).Distinct().ToListAsync();
        var types = await _context.Products.Select(x => x.Type).Distinct().ToListAsync();

        var filters = new { brands, types };
        
        // Salva no cache por 1 hora (filtros mudam pouco)
        await _cacheService.SetAsync(cacheKey, filters, TimeSpan.FromHours(1));

        return Ok(filters);
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
        {
            // Limpa o cache quando um produto é criado
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveByPatternAsync("products:filters");
            
            return CreatedAtAction(nameof(GetProduct), new { Id = product.Id }, product);
        }

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

        if (result) 
        {
            // Limpa o cache quando um produto é atualizado
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveAsync($"product:single:{product.Id}");
            await _cacheService.RemoveByPatternAsync("products:filters");
            
            return NoContent();
        }

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

        if (result) 
        {
            // Limpa o cache quando um produto é deletado
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveAsync($"product:single:{id}");
            await _cacheService.RemoveByPatternAsync("products:filters");
            
            return Ok();
        }

        return BadRequest("Problem deleting product");
    }
}
