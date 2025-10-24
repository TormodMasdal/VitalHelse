using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

// All of these actions demands a user, therefore, authorize the whole class
[Authorize]
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // Fetch the userId
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Finds the row in CartProducts where Product and userId match
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        // Removes the product
        if(shoppingCart != null)
            _db.Remove(shoppingCart);

        // Save the changes
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuantity(int id)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // Finds the row in CartProducts where Product and userId match 
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if (shoppingCart != null)
        {
            // Adds quantity by 1 and save it
            shoppingCart.Quantity += 1;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
    
    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DecreaseQuantity(int id)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // Finds the row in CartProducts where Product and userId match
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if (shoppingCart.Quantity <= 1)
        {
            // Returns a 400 bad request if the quantity is too low to use this function
            return BadRequest("Du må ha minst en gjenstand per produkt");
        }
        
        // Decrease the quantity by one
        shoppingCart.Quantity -= 1;
        await _db.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int id)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // If a user is signed in
        if (userId != null)
        {
            // Search for a matching row in the shoppingcart
            var shoppingCart = await _db.CartProducts
                .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

            // If the item already exists in the shoppingcart increment the quantity.
            if (shoppingCart != null)
            {
                shoppingCart.Quantity += 1;
                await _db.SaveChangesAsync();
            }
            
            // If not Create a new row and add the item
            else
            {
                // Creates a new row
                CartProduct cartProduct = new CartProduct
                {
                    Quantity = 1,
                    AspNetUsersId = userId,
                    ProductId = id
                };
                
                _db.Add(cartProduct);
                await _db.SaveChangesAsync();
            }
        }
        
        // Dont change the view
        return NoContent();
    }
}