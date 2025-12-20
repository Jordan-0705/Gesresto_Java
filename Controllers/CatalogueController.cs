using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace Gesresto.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly IBurgerService _burgerService;
        private readonly IMenuService _menuService;
        private readonly IComplementService _complementService;
        private readonly IClientService _clientService;

        public CatalogueController(IBurgerService burgerService,
                                   IMenuService menuService,
                                   IComplementService complementService,
                                   IClientService clientService)
        {
            _burgerService = burgerService;
            _menuService = menuService;
            _complementService = complementService;
            _clientService = clientService;
        }

        

        public async Task<IActionResult> Index(
            string filter = "Burger",
            string? search = null,
            int page = 1)
        {
            const int pageSize = 5;

            var model = new PagedCatalogueViewModel
            {
                Filter = filter,
                Search = search,
                CurrentPage = page,
                Burgers = new List<Burger>(),
                Menus = new List<Menu>(),
                Complements = new List<Complement>()
            };

            if (filter == "Burger")
            {
                var total = await _burgerService.GetAvailableCountAsync();
                model.Burgers = await _burgerService.GetPagedAsync(page, pageSize, search);
                model.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            }
            else if (filter == "Menu")
            {
                var total = await _menuService.GetAvailableCountAsync();
                model.Menus = await _menuService.GetPagedAsync(page, pageSize, search);
                model.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            }
            else
            {
                var total = await _complementService.GetAvailableCountAsync();
                model.Complements = await _complementService.GetPagedAsync(page, pageSize, search);
                model.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            }

            int clientId = int.Parse(User.FindFirst("ClientId").Value);
            var client = await _clientService.GetByIdAsync(clientId);

            ViewBag.UserName = client.Nom; 

            return View(model);
        }


    }
}
