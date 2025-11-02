using Microsoft.AspNetCore.Mvc;
using VitalHelse.Services;

namespace VitalHelse.Controllers;

public class TestController : Controller
{
    private readonly TripletexService _tripletex;

    public TestController(TripletexService tripletex)
    {
        _tripletex = tripletex;
    }

    [HttpGet("/test/tripletex")]
    public async Task<IActionResult> TestTripletex()
    {
        var session = await _tripletex.GetSessionTokenAsync();
        var products = await _tripletex.GetProductsAsync(session);
        return Json(products);
    }

    /*[HttpGet("/test/tripletex/details")]
    public async Task<IActionResult> TestTripletexDetails(int id = 69711757)
    {
        var session = await _tripletex.GetSessionTokenAsync();
        var json = await _tripletex.GetProductDetailsAsync(session, id);
        return Content(json, "application/json");
    }*/
}