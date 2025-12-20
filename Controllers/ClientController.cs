using Microsoft.AspNetCore.Mvc;
using Services;
using Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public IActionResult Index()
        {
            TempData.Clear();
            return View("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {

            TempData.Clear();
            
            int nextId = _clientService.GetNextId(); 
            string code = $"CLI-{nextId.ToString("D2")}"; 

            var model = new Client
            {
                Code = code
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(Client client)
        {
            if (!ModelState.IsValid) return View(client);

            if (await _clientService.ExistsLoginAsync(client.Login))
            {
                ModelState.AddModelError("Login", "Ce login est déjà utilisé");
                return View(client);
            }

            await _clientService.CreateAsync(client);

            TempData["Message"] = "Compte créé avec succès. Connectez-vous maintenant.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var client = await _clientService.AuthenticateAsync(login, password);

            if (client != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, client.Login),
                    new Claim("ClientId", client.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                return RedirectToAction("Index", "Accueil");
            }

            ViewBag.Error = "Login ou mot de passe incorrect.";
            return View("Login");
        }
    

        
        [HttpPost]
        [HttpGet] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData.Clear();
            
            
            TempData["Message"] = "Déconnexion réussie. À bientôt !";
            
            return RedirectToAction("Index"); 
        }
    }
}
