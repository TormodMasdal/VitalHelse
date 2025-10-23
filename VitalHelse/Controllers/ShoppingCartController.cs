using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

public class ShoppingCartController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<AspNetUsers> _userManager;

    public ShoppingCartController(ApplicationDbContext db, UserManager<AspNetUsers> userManager)
    {
        _db = db;
        _userManager = userManager;
    }
    
    // GET
    public IActionResult Index()
    {
        // Finds the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Query to get all items in the shopping cart
        var items = _db.CartProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.AspNetUsers)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToList();
        
        // Returns the items to the view
        return View(items);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if(shoppingCart != null)
            _db.Remove(shoppingCart);

        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPatch]
    public async Task<IActionResult> AddQuantity(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if (shoppingCart != null)
        {
            shoppingCart.Quantity += 1;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
    
    [HttpPatch]
    public async Task<IActionResult> DecreaseQuantity(int id)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if (shoppingCart.Quantity <= 1)
        {
            return BadRequest("Du må ha minst en gjenstand per produkt");
        }
        
        shoppingCart.Quantity -= 1;
        await _db.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }
}