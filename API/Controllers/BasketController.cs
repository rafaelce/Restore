using API.DTO;
using API.Entities;
using API.Extensions;
using API.Infra.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class BasketController(ApplicationContext context) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<BasketDto>> GetBasket()
    {
        var basket = await RetrieveBasket();
        if (basket is null) return NoContent();

        return basket.ToDto();
    }
    
    [HttpPost]
    public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        
        basket ??= CreateBasket();
        
        var product = await context.Products.FindAsync(productId);
        
        if (product is null) return BadRequest("Problem adding item to basket");
        
        basket.AddItem(product, quantity);
        
        var result = await context.SaveChangesAsync() > 0;
        
        if (result) return CreatedAtAction(nameof(GetBasket), basket.ToDto());
        
        return BadRequest("Problem updating basket");
    }

    [HttpDelete]
    public async Task<ActionResult<Basket>> RemoveItemFromBasket(int productId, int quantity)
    {
        var basket = await RetrieveBasket();
        
        if (basket is null) return BadRequest("Problem to retrieve basket");
        
        basket.RemoveItem(productId, quantity);
        
        var result = await context.SaveChangesAsync() > 0;
        
        if (result) return Ok();
        
        return BadRequest("Problem updating basket.");
    }
    
    private Basket CreateBasket()
    {
        var basketId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            IsEssential = true,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        Response.Cookies.Append("basketId", basketId, cookieOptions);
        var basket = new Basket {BasketId = basketId};
        context.Baskets.Add(basket);
        return basket;
    }
    
    private async Task<Basket?> RetrieveBasket()
    {
        return await context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.BasketId == Request.Cookies["basketId"]);
    }
}
