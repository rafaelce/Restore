using System.Text.Json;
using API.DTO;
using API.DTO.MassTransit;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Infra.EntityFramework;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class OrdersController(ApplicationContext context, IMessageService _messageService, ILogger<OrdersController> _logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders()
    {
       var orders = await context.Orders
            .ProjectToDto()
            .Where(x => x.BuyerEmail == User.GetUsername())
            .ToListAsync();
        
        return orders;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderDetails(int id)
    {
        var order = await context.Orders
            .ProjectToDto()
            .Where(x => x.BuyerEmail == User.GetUsername() && id == x.Id)
            .FirstOrDefaultAsync();

        if (order == null) return NotFound();

        return order;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var basket = await context.Baskets.GetBasketWithItems(Request.Cookies["basketId"]);

        if (basket == null || basket.Items.Count == 0 || string.IsNullOrEmpty(basket.PaymentIntentId))
            return BadRequest("Basket is empty or not found");

        var items = CreateOrderItems(basket.Items);

        if (items == null) return BadRequest("Some items out of stock");

        var subtotal = items.Sum(x => x.Price * x.Quantity);
        var deliveryFee = CalculateDeliveryFee(subtotal);

        var order = await context.Orders
        .Include(x => x.OrderItems)
        .FirstOrDefaultAsync(x => x.PaymentIntentId == basket.PaymentIntentId);

        if (order == null)
        {
            order = new Order
            {
                OrderItems = items,
                BuyerEmail = User.GetUsername(),
                ShippingAddress = orderDto.ShippingAddress,
                DeliveryFee = deliveryFee,
                Subtotal = subtotal,
                PaymentSummary = orderDto.PaymentSummary,
                PaymentIntentId = basket.PaymentIntentId
            };

            context.Orders.Add(order);

        }else
        {
            order.OrderItems = items;
        }

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Problem creating order");

        return CreatedAtAction(nameof(GetOrderDetails), new { id = order.Id }, order.ToDto());
    }

    [HttpPost("mass-transit")]
    public async Task<ActionResult> CreateOrderMassTransit()
    {
        try
        {
            // 1. Ler arquivo JSON de exemplo
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Infra", "DataTest", "sample-orders.json");
            
            if (!System.IO.File.Exists(jsonPath))
                return NotFound("Arquivo de exemplo não encontrado. Verifique se o arquivo sample-orders.json existe em Infra/DataTest/");
            

            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var sampleOrders = JsonSerializer.Deserialize<List<SampleOrderDto>>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (sampleOrders == null || !sampleOrders.Any())
                return BadRequest("Arquivo JSON inválido ou vazio");



            var results = new List<object>();
            var orderCounter = 0;

            foreach (var sampleOrder in sampleOrders)
            {
                orderCounter++;
                
                // 2. Simular criação de pedido (sem salvar no banco para demo)
                var orderId = DateTime.Now.Ticks + orderCounter; // ID único para demo
                
                _logger.LogInformation($"=== Processando Pedido #{orderId} ===");
                _logger.LogInformation($"Cliente: {sampleOrder.CustomerName} ({sampleOrder.CustomerEmail})");
                _logger.LogInformation($"Total: ${sampleOrder.TotalAmount}");
                _logger.LogInformation($"Itens: {sampleOrder.Items.Count}");

                // 3. Enviar mensagem para email de confirmação
                var orderMessage = new OrderCreatedMessage
                {
                    OrderId = (int)orderId,
                    CustomerEmail = sampleOrder.CustomerEmail,
                    CustomerName = sampleOrder.CustomerName,
                    TotalAmount = sampleOrder.TotalAmount,
                    CreatedAt = DateTime.UtcNow,
                    Items = sampleOrder.Items.Select(i => new OrderItemMessage
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                };

                await _messageService.PublishMessageAsync(orderMessage, "order-created");
                _logger.LogInformation($"✅ Mensagem de email enviada para fila");

                // 4. Enviar mensagens de atualização de estoque
                foreach (var item in sampleOrder.Items)
                {
                    var stockMessage = new StockUpdateMessage
                    {
                        ProductId = item.ProductId,
                        QuantitySold = item.Quantity,
                        UpdatedAt = DateTime.UtcNow
                    };
                                    await _messageService.PublishMessageAsync(stockMessage, "stock-update");
                _logger.LogInformation($"✅ Mensagem de estoque enviada para produto {item.ProductId}");
                }

                // 5. Enviar mensagem para geração de fatura
                var invoiceMessage = new InvoiceGenerationMessage
                {
                    OrderId = (int)orderId,
                    CustomerEmail = sampleOrder.CustomerEmail,
                    TotalAmount = sampleOrder.TotalAmount,
                    OrderDate = DateTime.UtcNow
                };
                await _messageService.PublishMessageAsync(invoiceMessage, "invoice-generation");
                _logger.LogInformation($"✅ Mensagem de fatura enviada para fila");

                // 6. Enviar notificação push
                var pushMessage = new PushNotificationMessage
                {
                    CustomerId = sampleOrder.CustomerId,
                    Title = "Pedido Confirmado!",
                    Message = $"Seu pedido #{orderId} foi confirmado e está sendo processado.",
                    OrderId = orderId.ToString()
                };
                await _messageService.PublishMessageAsync(pushMessage, "push-notification");
                _logger.LogInformation($"✅ Mensagem de push notification enviada");

                results.Add(new
                {
                    OrderId = orderId,
                    CustomerName = sampleOrder.CustomerName,
                    CustomerEmail = sampleOrder.CustomerEmail,
                    TotalAmount = sampleOrder.TotalAmount,
                    ItemsCount = sampleOrder.Items.Count,
                    MessagesSent = 3 + sampleOrder.Items.Count, // email + fatura + push + estoque por item
                    Status = "Processado com sucesso"
                });

                _logger.LogInformation($"=== Pedido #{orderId} processado ===\n");
            }

            return Ok(new
            {
                Message = $"Processados {sampleOrders.Count} pedidos de exemplo",
                TotalMessagesSent = results.Count * 4, // Aproximadamente 4 mensagens por pedido
                Orders = results,
                RabbitMQStatus = "Mensagens enviadas para processamento assíncrono"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pedidos de exemplo");
            return StatusCode(500, new { Error = "Erro interno do servidor", Details = ex.Message });
        }
    }

    private long CalculateDeliveryFee(long subtotal)
    {
        return subtotal > 10000 ? 0 : 500;
    }

    private List<OrderItem>? CreateOrderItems(List<BasketItem> items)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            if (item.Product.QuantityInStock < item.Quantity)
                return null;

            var orderItem = new OrderItem
            {
                ItemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    PictureUrl = item.Product.PictureUrl,
                    Name = item.Product.Name
                },
                Price = item.Product.Price,
                Quantity = item.Quantity
            };

            orderItems.Add(orderItem);

            item.Product.QuantityInStock -= item.Quantity;
        } 

        return orderItems;
    }
}
