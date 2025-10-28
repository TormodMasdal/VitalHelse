using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VitalHelse.Configuration;
using VitalHelse.Data;
namespace VitalHelse.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IOptions<StripeOptions> _stripeOptions;

    public CheckoutController(ApplicationDbContext db, IOptions<StripeOptions> stripeOptions)
    {
        _db = db;
        _stripeOptions = stripeOptions;
    }

    [HttpPost("create-session")]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var cartItems = await _db.CartProducts
            .Include(cp => cp.Product)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();

        var productService = new ProductService();
        var priceService = new PriceService();
        var lineItems = new List<SessionLineItemOptions>();

        foreach (var item in cartItems)
        {
            var product = item.Product;

            if (string.IsNullOrEmpty(product.StripeProductId))
            {
                var stripeProduct = await productService.CreateAsync(new ProductCreateOptions
                {
                    Name = product.ProductName
                });
                product.StripeProductId = stripeProduct.Id;
            }

            if (string.IsNullOrEmpty(product.StripePriceId))
            {
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

        var host = $"{Request.Scheme}://{Request.Host}";
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{host}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{host}/canceled.html",
            Mode = "payment",
            LineItems = lineItems
        };

        var session = await new SessionService().CreateAsync(options);
        return Redirect(session.Url);
    }
}

