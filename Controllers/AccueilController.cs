using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace Controllers
{
    public class AccueilController : Controller
    {
        
        private readonly IClientService _clientService;

        public AccueilController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            int clientId = int.Parse(User.FindFirst("ClientId").Value);
            var client = await _clientService.GetByIdAsync(clientId);

            ViewBag.UserName = client.Nom; 
            return View();
        }
    }
}
