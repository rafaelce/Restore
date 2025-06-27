using API.DTO;
using API.Extensions;
using API.Infra.EntityFramework;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PaymentsController(PaymentsService paymentsService, ApplicationContext context) : BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent()
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);

        if(basket == null) return BadRequest(new { error = "Problem with basket" });

        var intent = await paymentsService.CreateOrUpdatePaymentIntent(basket);

        if(intent == null) return BadRequest("Problem creating payment intent");

        basket.PaymentIntentId ??= intent.Id;
        basket.ClientSecret ??= intent.ClientSecret;

        if(context.ChangeTracker.HasChanges()){
             var result = await context.SaveChangesAsync() > 0;
              if(!result) return BadRequest("Problem updating basket with intent");
        }

        return basket.ToDto();
    }
}
