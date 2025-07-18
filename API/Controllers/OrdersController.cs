using System.Text.Json;
using API.DTO;
using API.DTO.MassTransit;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Events;
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

    [HttpPost("kafka-demo")]
    public async Task<ActionResult> CreateKafkaDemo()
    {
        try
        {
            var kafkaService = HttpContext.RequestServices.GetRequiredService<IKafkaService>();

            // Gerar IDs únicos baseados no timestamp
            var timestamp = DateTime.UtcNow.Ticks;
            var userId = $"user-{timestamp % 10000}";
            var orderId = $"order-{timestamp % 100000}";
            var sessionId = $"session-{timestamp % 1000}";

            // Lista de produtos para escolha aleatória
            var products = new[] { "boot-redis1", "boot-core1", "hat-react1", "glove-code1", "sb-ang1" };
            var random = new Random();
            var selectedProduct = products[random.Next(products.Length)];

            // Lista de termos de busca para escolha aleatória
            var searchTerms = new[] { "redis", "react", "angular", "core", "typescript", "javascript" };
            var selectedSearchTerm = searchTerms[random.Next(searchTerms.Length)];

            // Lista de categorias para escolha aleatória
            var categories = new[] { "books", "electronics", "clothing", "sports", "home" };
            var selectedCategory = categories[random.Next(categories.Length)];

            // Simular eventos de usuário
            var userEvent = new UserEvent
            {
                UserId = userId,
                EventType = "product_view",
                ProductId = selectedProduct,
                PageUrl = $"/products/{selectedProduct}",
                Timestamp = DateTime.UtcNow,
                AdditionalData = new Dictionary<string, object>
                {
                    { "sessionId", sessionId },
                    { "userAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36" },
                    { "referrer", "https://www.google.com" }
                }
            };

            await kafkaService.PublishUserEventAsync(userEvent);

            // Simular evento de pedido
            var orderEvent = new OrderEvent
            {
                OrderId = orderId,
                UserId = userId,
                EventType = "order_created",
                TotalAmount = (decimal)(random.Next(50, 500) + random.NextDouble()),
                ProductIds = new List<string> { selectedProduct, products[random.Next(products.Length)] },
                Timestamp = DateTime.UtcNow,
                AdditionalData = new Dictionary<string, object>
                {
                    { "paymentMethod", random.Next(2) == 0 ? "credit_card" : "paypal" },
                    { "shippingAddress", $"{random.Next(100, 999)} Main St" },
                    { "discountApplied", random.Next(2) == 0 }
                }
            };

            await kafkaService.PublishOrderEventAsync(orderEvent);

            // Simular evento de busca
            var searchEvent = new SearchEvent
            {
                UserId = userId,
                SearchTerm = selectedSearchTerm,
                Category = selectedCategory,
                ResultsCount = random.Next(5, 50),
                Timestamp = DateTime.UtcNow,
                AdditionalData = new Dictionary<string, object>
                {
                    { "filters", random.Next(2) == 0 ? "price:asc" : "price:desc" },
                    { "page", random.Next(1, 5) },
                    { "sortBy", random.Next(2) == 0 ? "relevance" : "date" }
                }
            };

            await kafkaService.PublishSearchEventAsync(searchEvent);

            return Ok(new
            {
                message = "Kafka events published successfully",
                events = new
                {
                    userEvent = new
                    {
                        userId = userEvent.UserId,
                        eventType = userEvent.EventType,
                        productId = userEvent.ProductId
                    },
                    orderEvent = new
                    {
                        orderId = orderEvent.OrderId,
                        userId = orderEvent.UserId,
                        eventType = orderEvent.EventType,
                        totalAmount = orderEvent.TotalAmount
                    },
                    searchEvent = new
                    {
                        userId = searchEvent.UserId,
                        searchTerm = searchEvent.SearchTerm,
                        category = searchEvent.Category,
                        resultsCount = searchEvent.ResultsCount
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
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
