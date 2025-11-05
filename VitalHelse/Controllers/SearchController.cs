using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalHelse.Data;
using VitalHelse.Models;

namespace VitalHelse.Controllers;

/// <summary>
/// SearchController
/// Inherits from controller class
/// Contains methods for searching products
/// </summary>
public class SearchController : Controller
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a list of suggested products based on a search term.
    /// </summary>
    /// <param name="term">
    /// A string representing the search term used to match product names or descriptions.
    /// </param>
    /// <param name="maxResults">
    /// An optional integer specifying the maximum number of suggestions to return. Default value is 10.
    /// </param>
    /// <returns>
    /// An asynchronous result containing a JSON object with a list of product suggestions.
    /// Each suggestion includes product ID, name, truncated description, and price.
    /// If the search term is null, empty, or whitespace, an empty JSON list is returned.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> SearchSuggestions(string term, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Json(new List<object>());
        }
        
        term = term.ToLower();

        var suggestions = await _context.Products
            .Where(p =>
                (p.ProductName != null && p.ProductName.ToLower().Contains(term)) ||
                (p.ProductDescription != null && p.ProductDescription.ToLower().Contains(term))
            )
            .Take(maxResults)
            //Transforms the result set into a list of objects with the following properties:
            .Select(p => new
            {
                id = p.ProductId,
                name = p.ProductName,
                description = p.ProductDescription != null && p.ProductDescription.Length > 50 
                    ? p.ProductDescription.Substring(0, 50) + "..." 
                    : p.ProductDescription,
                price = p.ProductPriceInVAT
            })
            .ToListAsync();
        //Returns the list of suggestions as a JSON object
        return Json(suggestions);
    }

    /// <summary>
    /// Retrieves a list of products based on a search term.
    /// </summary>
    /// <param name="query">
    /// A string representing the search term used to match product names or descriptions.
    /// </param>
    /// <returns>
    /// An asynchronous result containing a view with a list of products.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return View(new List<Product>());
        }

        var products = await _context.Products
            .Include(p => p.ProductPictures)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Where(p =>
                (p.ProductName != null && p.ProductName.Contains(query)) ||
                (p.ProductDescription != null && p.ProductDescription.Contains(query))
            )
            .ToListAsync();

        ViewData["SearchQuery"] = query;
        return View(products);
    }
}