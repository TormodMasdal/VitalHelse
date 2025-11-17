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
        if (shoppingCart != null)
            _db.Remove(shoppingCart);

        // Save the changes
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuantity(int id)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Finds the row in CartProducts where Product and userId match 
        var shoppingCart = await _db.CartProducts
            .Include(c => c.Product)
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        if (shoppingCart != null)
        {
            if (shoppingCart.Quantity >= shoppingCart.Product.StockCount)
            {
                // Returns a 400 bad request if the quantity equals the amount of stock
                return BadRequest("Vi har desverre ikke dette antaller tilgjengelig på lager");
            }

            // Adds quantity by 1 and save it
            shoppingCart.Quantity += 1;
            await _db.SaveChangesAsync();
        }
        
        return NoContent();
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
            _db.CartProducts.Remove(shoppingCart); 
            await _db.SaveChangesAsync();
            return NoContent();  
        }

        // Decrease the quantity by one
        shoppingCart.Quantity -= 1;
        await _db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPatch]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetQuantity(int id, int quantity)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Finds the row in CartProducts where Product and userId match
        var shoppingCart = await _db.CartProducts
            .Include(c => c.Product)
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        // If cart item doesnt exist
        if (shoppingCart == null)
            return NotFound();
        
        // If quantity is less than zero or zero
        if (quantity <= 0)
            return BadRequest("Antall må være minst 1.");
        
        // If stock is NULL (not set) then use 0
        var stock = shoppingCart.Product.StockCount ?? 0;
        
        // If quantity is more than stock then set quantity to stock 
        if (quantity > stock)
            quantity = stock;
        // If quantity is more than stock then set quantity to stock 
        if (quantity > shoppingCart.Product.StockCount.GetValueOrDefault())
            quantity = shoppingCart.Product.StockCount.GetValueOrDefault();
        
        // Update the quantity 
        shoppingCart.Quantity = quantity;
        await _db.SaveChangesAsync();
        
        // Return a JSON object with the adjusted quantity
        return Json(new { correctedQuantity = quantity }); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddToCart(int id, int quantity)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Search for a matching row in the shoppingcart
        var shoppingCart = await _db.CartProducts
            .FirstOrDefaultAsync(m => m.ProductId == id && m.AspNetUsersId == userId);

        // Gets the productId, so that we can match the cart quantity with stock count
        var product = await _db.Products
            .FirstOrDefaultAsync(m => m.ProductId == id);

        if (product == null) return NoContent();

        // The stock count has to be higher than 0 to add the item to cart.
        if (product.StockCount <= 0) return NoContent();
        
        // If the item already exists in the shoppingcart increment the quantity.
        if (shoppingCart != null)
        { 
            var newQuantity = shoppingCart.Quantity + quantity;
            if (newQuantity >= product.StockCount)
            {
                return BadRequest("Vi har desverre ikke dette antallet tilgjengelig på lager");
            } 
            
            /*
            // If the quantity tries to go higher than the stock count
            if (shoppingCart.Quantity >= product.StockCount)
            {
                // Returns a 400 bad request if the quantity is too low to use this function
                return BadRequest("Vi har desverre ikke dette antallet tilgjengelig på lager");
            } */
            
            shoppingCart.Quantity = newQuantity;

           // shoppingCart.Quantity += 1; 
        }

        // If not. Create a new row and add the item
        else
        {
            if (quantity > product.StockCount)
                return BadRequest("Vi har desverre ikke dette antallet tilgjengelig på lager");
            
            // Creates a new row
            CartProduct cartProduct = new CartProduct
            {
                Quantity = quantity,
                //Quantity = 1,
                AspNetUsersId = userId,
                ProductId = id
            };

            _db.Add(cartProduct);
        }

        await _db.SaveChangesAsync();
        

        // Dont change the view
        return NoContent();
    }

        
    [HttpGet]
    public async Task<IActionResult> Summary()
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // Get all the items in the shopping cart
        var items = await _db.CartProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.AspNetUsers)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();
        
        // If shopping cart is empty then return 0 values
        if (!items.Any())
        {
            return Json(new { sumProducts = 0, sumBefore = 0, discount = 0, shipping = 0, total = 0 });
        }
        
        // midlertidig bruk av decimal pga mulige endringer i modellen 
        
        decimal sumBefore = items.Sum(i => (decimal)i.Product.ProductPriceInVAT * i.Quantity);
        decimal sumProducts = items.Sum(i => (decimal)(i.Product.ProductCampaignPrice ?? i.Product.ProductPriceInVAT) * i.Quantity);
        decimal discount = sumBefore - sumProducts;
        decimal shipping = 0; 
        decimal total= sumProducts + shipping;

        // Return all the values as JSON
        return Json(new { sumProducts, sumBefore, discount, shipping, total });
    }    
    
    public async Task<IActionResult> Shipping(){
        return View();
    }
    
    public async Task<IActionResult> Review(){
        return View();
    }
    
    public async Task<IActionResult> Complete(){
        return View();
    }
    
    [HttpGet]
    public async Task<IActionResult> Address()
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Query to get all the users Addresses
        var list = await _db.Addresses
            .Where(a => a.AspNetUsersId == userId)
            .OrderByDescending(a => a.AddressId)
            .ToListAsync();

        // Add the addresses in a Viewbag
        ViewBag.Addresses = list;
        // Decide which address is going to be selected, if SelectedAddressId exists then use, if not use the first in the list
        ViewBag.SelectedAddressId = (TempData["SelectedAddressId"] as int?) ?? list.FirstOrDefault()?.AddressId;

        return View("Address", new Address());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress([Bind("Street,City,PostalCode")] Address form)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Check if model is valid
        if (!ModelState.IsValid)
        {
            ViewBag.Addresses = await _db.Addresses
                .Where(a => a.AspNetUsersId == userId)
                .OrderByDescending(a => a.AddressId)
                .ToListAsync();
            
            return View("Address", form);
        }
        
        form.AspNetUsersId = userId;
        
        _db.Add(form);
        await _db.SaveChangesAsync();

        TempData["SelectedAddressId"] = form.AddressId;
        return RedirectToAction(nameof(Address));
    }
}