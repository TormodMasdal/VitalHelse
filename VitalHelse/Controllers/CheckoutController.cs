using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;
using VitalHelse.Models.Enums;

namespace VitalHelse.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<AspNetUsers> _userManager;

    public CheckoutController(ApplicationDbContext db, UserManager<AspNetUsers> userManager)
    {
        _db = db;
        _userManager = userManager;
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
    
    public async Task<IActionResult> Index()
    {
        ViewBag.Step = CheckoutStep.Cart;
        
        // Finds the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Query to get all items in the shopping cart
        var items = await _db.CartProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.Product.ProductPictures)
            .Include(cp => cp.AspNetUsers)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();

        // Returns the items to the view
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BeginCheckout()
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // Fetch all the items from the cart
        var cart = await _db.CartProducts
            .Include(cp => cp.Product)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();
        
        // If cart is empty
        if (!cart.Any())
        {
            // HUSK: fiks error melding
            return RedirectToAction(nameof(Index));
        }

        // Remove old draft (fikser senere at carten blir bare oppdatert)
        var oldDrafts = await _db.Orders
            .Where(o => o.AspNetUsersId == userId && o.Status == "Draft")
            .ToListAsync();
        _db.Orders.RemoveRange(oldDrafts);

        // Create a new order
        var order = new Order
        {
            AspNetUsersId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Draft",
        };

        // Add all cart products in order
        foreach (var item in cart)
        {
            order.OrderProducts.Add(new OrderProduct
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = (decimal)(item.Product.ProductCampaignPrice ?? item.Product.ProductPriceInVAT)
            });
        }
        
        // Save total cost
        order.TotalCost = order.OrderProducts.Sum(op => op.UnitPrice * op.Quantity);

        // Save and add to database
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Address));
    }
    
    
    
    [HttpGet]
    public async Task<IActionResult> Address()
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Create a viewmodel and add user addresses
        var vm = new UserAddressViewModel
        {
            Existing = await _db.UserAddresses
                .Include(a => a.AspNetUsers )
                .Where(a => a.AspNetUserId == userId)
                .OrderByDescending(a => a.Id)
                .ToListAsync()
        };

        ViewBag.Step = CheckoutStep.Address;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress(UserAddressViewModel vm)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // If model is invalid (missing input)
        if (!ModelState.IsValid)
        {
            vm.Existing = await _db.UserAddresses.Where(a => a.AspNetUserId == userId).ToListAsync();
            ViewBag.Step = CheckoutStep.Address;
            return View("Address", vm);
        }
    
        // Save the new values and add to database
        vm.NewAddress.AspNetUserId = userId!;
        vm.NewAddress.AspNetUsers =  await _userManager.GetUserAsync(User);
        _db.UserAddresses.Add(vm.NewAddress);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Address));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SelectAddress(UserAddressViewModel vm)
    {
        // Fetch the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        // If none addresses are selected
        if (vm.SelectedAddressId == null)
        {
            var userIdForError = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            vm.Existing = await _db.UserAddresses
                .Include(a => a.AspNetUsers)
                .Where(a => a.AspNetUserId == userIdForError)
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            ViewBag.Step = CheckoutStep.Address;
            return RedirectToAction(nameof(Address));
        }
        
        // Find the users order
        var order = await _db.Orders
            .Where(o => o.AspNetUsersId == userId && o.Status == "Draft")
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return RedirectToAction(nameof(Index));
        }
        // Save the selected id
        var selectedId = vm.SelectedAddressId.Value;
        
        order.UserAddress = await _db.UserAddresses.FindAsync(selectedId);

        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Shipping));
    }
    
    
    
    public async Task<IActionResult> Shipping(){
        ViewBag.Step = CheckoutStep.Shipping;
        
        return View();
    }
    
    public async Task<IActionResult> ReviewOrder(){
        ViewBag.Step = CheckoutStep.ReviewOrder;
        
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (userId == null)
        {
            return NoContent();
        }
        
        var order = await _db.Orders
            .Include(o => o.UserAddress)
            .Include(o => o.AspNetUsers )
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product)
            .Include(o => o.OrderProducts)
            .ThenInclude(op => op.Product.ProductPictures)
            .Where(o => o.AspNetUsersId == userId && o.Status == "Draft")
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            // HUSK: Legge til error
            return RedirectToAction("Index", "Checkout");
        }

        // Add data to the view model
        var vm = new ReviewOrderViewModel
        {
            UserAddress = order.UserAddress ?? new UserAddress(),
            CartProducts = order.OrderProducts.Select(op => new CartProduct
            {
                ProductId = op.ProductId,
                Quantity = op.Quantity,
                Product = op.Product
            }).ToList()
        };

        return View(vm);
    }
    
    public async Task<IActionResult> Complete(){
        ViewBag.Step = CheckoutStep.Complete;
        return View();
    }
    
    // View for the orders page
    public async Task<IActionResult> Orderss()
    {
        var orders = await _db.Orders
            .Include(o => o.AspNetUsers)
            .Include(o => o.UserAddress)
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

}