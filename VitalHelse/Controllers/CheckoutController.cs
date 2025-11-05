using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



using VitalHelse.Configuration;
using VitalHelse.Data;
namespace VitalHelse.Controllers;

[Route("create-checkout-session")]
[ApiController]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IOptions<StripeOptions> _stripeOptions;

    public CheckoutController(ApplicationDbContext db, IOptions<StripeOptions> stripeOptions)
    {
        _db = db;
        _stripeOptions = stripeOptions;
    }

    /// <summary>
    /// Creates a checkout session for the logged-in user.
    /// Takes the products and quantities from the shopping cart,
    /// checks their prices, and includes them in the checkout.
    /// </summary>

    /// <returns> A checkout page</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        // Finds the user id of the user creating the checkout session
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Fetch all the items from the cart
        var cartItems = await _db.CartProducts
            .Include(cp => cp.Product)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();

        var productService = new ProductService();
        var priceService = new PriceService();
        var lineItems = new List<SessionLineItemOptions>();

        // For every item in the cart
        foreach (var item in cartItems)
        {
            var product = item.Product;

            // If the product doesn't exist on the owners stripe account, create a new one
            if (string.IsNullOrEmpty(product.StripeProductId))
            {
                var stripeProduct = await productService.CreateAsync(new ProductCreateOptions
                {
                    Name = product.ProductName
                });
                product.StripeProductId = stripeProduct.Id;
            }

            // If the product price doesn't exist on the owners stripe account, create a new one
            if (string.IsNullOrEmpty(product.StripePriceId))
            {
                // Checks if the price is on campaign, if so use the campaign price, else use the normal price
                var effectivePrice = product.ProductCampaignPrice ?? product.ProductPriceInVAT;
                var stripePrice = await priceService.CreateAsync(new PriceCreateOptions
                {
                    UnitAmount = (long)(effectivePrice * 100),
                    Currency = "nok",
                    Product = product.StripeProductId,
                });
                product.StripePriceId = stripePrice.Id;
            }
            
            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = item.Quantity,
                Price = product.StripePriceId
            });
        }

        await _db.SaveChangesAsync();

        // Creates checkout details
        var host = $"{Request.Scheme}://{Request.Host}";
        var options = new SessionCreateOptions
        {
            // Makes the checkout embedded to our website
            UiMode = "embedded",
            Mode = "payment",
            LineItems = lineItems,
            ReturnUrl = host + "/return.html?session_id={CHECKOUT_SESSION_ID}",
            
            // Saves the userId for the webhook later
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId }
                }
            }
        };

        var service = new SessionService();
        
        // Create the session with the given details (options)
        Session session = service.Create(options);

        return Json(new { clientSecret = session.ClientSecret });
    }
}

/// <summary>
/// Gets confirmation from stripe when purchase confirmed
/// Not safe to create orders, may be manipulated
/// </summary>
[Route("session-status")]
[ApiController]
public class SessionStatusController : Controller
{
    [HttpGet]
    public ActionResult SessionStatus([FromQuery] string session_id)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(session_id);

        return Json(new {status = session.Status,  customer_email = session.CustomerDetails.Email});
    }
}

