using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using VitalHelse.Data;
using VitalHelse.Models;


[Route("api/[controller]")]
[ApiController]
public class StripeWebHook : Controller
{
    private readonly ApplicationDbContext _db;
    private const string EndpointSecret = "whsec_LlSZSSIpI4uXFAZFFKFgSR3vOTNbzJOl"; 

    public StripeWebHook(ApplicationDbContext db)
    {
        _db = db;
    }
        
    /// <summary>
    /// Webhook, only implemented when purchase is completed, may be changed later.
    /// When purchase is completed, remove all items from the shopping cart and create a new order
    /// </summary>
    /// <returns>A new order</returns>
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Event stripeEvent;

        try
        {
            // Verify signature, safety against false webhooks
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                EndpointSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException e)
        {
            Console.WriteLine($"Stripe error: {e.Message}");
            return BadRequest();
        }

        // Event when the payment is succeeded
        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null)
                return Ok();

            if (!paymentIntent.Metadata.TryGetValue("userId", out var userId))
            {
                Console.WriteLine("Missing userId in payment metadata");
                return Ok();
            }
            
            var paymentIntentId = paymentIntent.Id;

            // Test to avoid duplicate order in case of network error
            var existingOrder = await _db.Orders
                .FirstOrDefaultAsync(o => o.StripePaymentIntentId == paymentIntentId);

            if (existingOrder != null)
            {
                Console.WriteLine($"Order already exists for PaymentIntent {paymentIntentId}");
                return Ok();
            }

            Console.WriteLine($"Payment succeeded for user {userId}");

            // Create order
            var order = new Order
            { 
                AspNetUsersId = userId, 
                OrderDate = DateTime.UtcNow,
                StripePaymentIntentId = paymentIntentId
            };

            // Fetch shopping cart from the database
            var cartItems = await _db.CartProducts
                .Include(cp => cp.Product)
                .Where(cp => cp.AspNetUsersId == userId)
                .ToListAsync();
            // Move items in shoppingcart into order table
            foreach (var item in cartItems)
            { 
                order.OrderProducts.Add(new OrderProduct {
                    ProductId = item.Product.ProductId,
                    Quantity = item.Quantity
                });
                
                item.Product.StockCount -= item.Quantity;
            }
            // Adds order, and removes the shopping cart
            _db.Orders.Add(order);
            _db.CartProducts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            Console.WriteLine($"Created order {order.OrderId} for user {userId}");
        }
        else
        { 
            Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
        }
        return Ok();
    }
}

