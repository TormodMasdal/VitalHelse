using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AspNetUsers> _um;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(
            ApplicationDbContext db,
            UserManager<AspNetUsers> um,
            ILogger<FavoritesController> logger)
        {
            _db = db;
            _um = um;
            _logger = logger;
        }

        [HttpGet("/favoritter")]
        public async Task<IActionResult> Favorites()
        {
            var user = await _um.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var favoriteProducts = await _db.FavoriteProducts
                .Include(fp => fp.Product)
                .Where(fp => fp.AspNetUsersId == user.Id) // 👈 bruker-ID matcher nå IdentityUser
                .Select(fp => fp.Product)
                .ToListAsync();

            var viewModel = new FavoritesViewModel
            {
                Products = favoriteProducts ?? new List<Product>()
            };

            return View("FavoritesPage", viewModel);
        }
    }
}