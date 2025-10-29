using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitalHelse.Models;

namespace VitalHelse.Controllers
{
    /// <summary>
    /// Handles requests related to the main pages of the application such as
    /// the home page, privacy policy, and global error handling.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for recording diagnostic and error information.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the main landing page of the website.
        /// </summary>
        /// <returns>The home page view.</returns>
        public IActionResult Index()
        {
            _logger.LogInformation("Home page accessed.");
            return View();
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>The privacy policy view.</returns>
        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy page accessed.");
            return View();
        }

        /// <summary>
        /// Displays a detailed error page when an unhandled exception occurs.
        /// </summary>
        /// <remarks>
        /// The response is not cached and includes diagnostic information such as the request ID.
        /// </remarks>
        /// <returns>An error view with diagnostic details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Unhandled error occurred. Request ID: {RequestId}", requestId);

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}