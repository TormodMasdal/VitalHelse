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
}