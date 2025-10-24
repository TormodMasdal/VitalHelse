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
        
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var user = await _um.GetUserAsync(User);
            if (user == null) return Challenge();

            var existing = await _db.FavoriteProducts
                .FirstOrDefaultAsync(f => f.AspNetUsersId == user.Id && f.ProductId == productId);

            if (existing == null)
            {
                _db.FavoriteProducts.Add(new FavoriteProduct
                {
                    AspNetUsersId = user.Id,
                    ProductId = productId
                });
            }
            else
            {
                _db.FavoriteProducts.Remove(existing);
            }

            await _db.SaveChangesAsync();

            // 👇 Går tilbake til siden brukeren kom fra
            return Json(new { success = true, isFavorite = existing == null });

        }

    }
}