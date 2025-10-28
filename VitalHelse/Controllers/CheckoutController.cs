using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using VitalHelse.Configuration;


namespace VitalHelse.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly StripeOptions _stripeOptions;

    public CheckoutController(IOptions<StripeOptions> stripeOptions)
    {
        _stripeOptions = stripeOptions.Value;
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromForm] long quantity)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{_stripeOptions.Domain}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_stripeOptions.Domain}/canceled.html",
            Mode = "payment",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = quantity,
                    Price = _stripeOptions.Price,
                },
            },
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return Ok(new { url = session.Url });
    }
}
