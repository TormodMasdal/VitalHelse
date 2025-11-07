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
    
    // GET
    public async Task<IActionResult> Index()
    {
        ViewBag.Step = CheckoutStep.Cart;
        
        // Finds the user id
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Query to get all items in the shopping cart
        var items = _db.CartProducts
            .Include(cp => cp.Product)
            .Include(cp => cp.Product.ProductPictures)
            .Include(cp => cp.AspNetUsers)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToList();

        // Returns the items to the view
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BeginCheckout()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        
        // Fetch all the items from the cart
        var cart = await _db.CartProducts
            .Include(cp => cp.Product)
            .Where(cp => cp.AspNetUsersId == userId)
            .ToListAsync();

        if (!cart.Any())
        {
            TempData["CheckoutError"] = "Handlekurven er tom.";
            return RedirectToAction(nameof(Index));
        }

        // Slett eventuell tidligere draft
        var oldDrafts = await _db.Orders
            .Where(o => o.AspNetUsersId == userId && o.Status == "Draft")
            .ToListAsync();
        _db.Orders.RemoveRange(oldDrafts);

        // Lag ny ordre (draft) + lines
        var order = new Order
        {
            AspNetUsersId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Draft",
        };

        foreach (var item in cart)
        {
            order.OrderProducts.Add(new OrderProduct
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = (decimal)(item.Product.ProductCampaignPrice ?? item.Product.ProductPriceInVAT)
            });
        }

        order.TotalCost = order.OrderProducts.Sum(op => op.UnitPrice * op.Quantity);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        
        return RedirectToAction(nameof(Address));
    }
    
    
    
    
    [HttpGet]
    public async Task<IActionResult> Address()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var vm = new UserAddressViewModel
        {
            Existing = await _db.UserAddresses
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
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!ModelState.IsValid)
        {
            // Husk å fylle Existing igjen når validering feiler
            vm.Existing = await _db.UserAddresses.Where(a => a.AspNetUserId == userId).ToListAsync();
            ViewBag.Step = CheckoutStep.Address;
            return View("Address", vm);
        }

        vm.NewAddress.AspNetUserId = userId!;
        _db.UserAddresses.Add(vm.NewAddress);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Address)); // PRG
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UseAddress(UserAddressViewModel vm)
    {
        if (vm.SelectedAddressId == null)
        {
            // Ingen valgt – gå tilbake med feilmelding
            ModelState.AddModelError(nameof(vm.SelectedAddressId), "Velg en adresse.");
            return RedirectToAction(nameof(Address));
        }

        var selectedId = vm.SelectedAddressId.Value;

        
        return NoContent();
    }
    
    public async Task<IActionResult> Shipping(){
        ViewBag.Step = CheckoutStep.Shipping;
        
        return View();
    }
    
    public async Task<IActionResult> Payment(){
        ViewBag.Step = CheckoutStep.Payment;
        
        return View();
    }
    
    public async Task<IActionResult> Complete(){
        ViewBag.Step = CheckoutStep.Complete;
        return View();
    }
    
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