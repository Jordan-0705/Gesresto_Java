using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gesresto.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Gesresto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GesRestoDbContext _context; // <-- ajout du DbContext

        public HomeController(ILogger<HomeController> logger, GesRestoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // ===== Route de test de connexion à la DB =====
        public IActionResult TestDb()
        {
            try
            {
                // Exemple : compter le nombre de clients
                var count = _context.Clients?.Count() ?? 0;
                return Content($"Connexion OK ! Nombre de clients : {count}");
            }
            catch (Exception ex)
            {
                return Content($"Erreur de connexion : {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
