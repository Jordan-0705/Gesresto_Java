// CommandeController.cs - VERSION FINALE CORRIGÉE
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;
using Services;
using GesResto.ViewModels;
using System.Text.Json;

[Authorize]
public class CommandeController : Controller
{
    private readonly IBurgerService _burgerService;
    private readonly IMenuService _menuService;
    private readonly IZoneService _zoneService;
    private readonly IComplementService _complementService;
    private readonly GesRestoDbContext _context;

    public CommandeController(
        IBurgerService burgerService,
        IMenuService menuService,
        IZoneService zoneService,
        IComplementService complementService,
        GesRestoDbContext context)
    {
        _burgerService = burgerService;
        _menuService = menuService;
        _zoneService = zoneService;
        _complementService = complementService;
        _context = context;
    }

    public async Task<IActionResult> PasserCommande()
    {
        ViewBag.Zones = await _zoneService.GetAllZonesAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FormulaireCommande(string CommandeType, int ZoneId, string ConsoType)
    {
        ViewBag.CommandeType = CommandeType;
        ViewBag.ZoneId = ZoneId;
        ViewBag.ConsoType = ConsoType;
        
        if (CommandeType == "Burger")
        {
            ViewBag.Burgers = await _burgerService.GetAllAsync();
        }
        else if (CommandeType == "Menu")
        {
            ViewBag.Menus = await _menuService.GetAllAsync();
        }
        
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SelectionComplements()
    {
        var commandeType = TempData["CommandeType"] as string;
        var zoneIdStr = TempData["ZoneId"] as string;
        var consoType = TempData["ConsoType"] as string;
        
        if (string.IsNullOrEmpty(commandeType) || string.IsNullOrEmpty(zoneIdStr))
        {
            TempData["Error"] = "Session expirée. Veuillez recommencer.";
            return RedirectToAction("PasserCommande");
        }
   
        ViewBag.CommandeType = commandeType;
        ViewBag.ZoneId = int.Parse(zoneIdStr);
        ViewBag.ConsoType = consoType;
 
        TempData.Keep("CommandeType");
        TempData.Keep("ZoneId");
        TempData.Keep("ConsoType");
        TempData.Keep("SelectedBurgers");
        TempData.Keep("SelectedMenus");
   
        var frites = await _complementService.GetByTypeAsync(ComplementType.Frites);
        var boissons = await _complementService.GetByTypeAsync(ComplementType.Boisson);
        
        ViewBag.Frites = frites;
        ViewBag.Boissons = boissons;
        
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var commande = await _context.Commandes
            .Include(c => c.Client)
            .Include(c => c.Zone)
            .Include(c => c.CommandeBurgers)
                .ThenInclude(cb => cb.Burger)
            .Include(c => c.CommandeMenus)
                .ThenInclude(cm => cm.Menu)
                    .ThenInclude(m => m.Burger)
            .Include(c => c.CommandeMenus)
                .ThenInclude(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementF)
            .Include(c => c.CommandeMenus)
                .ThenInclude(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementB)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (commande == null)
        {
            return NotFound();
        }

        return View(commande);
    }

    [HttpGet]
    public IActionResult Annuler()
    {
        return RedirectToAction("Index", "Catalogue");
    }

    [HttpPost]
    public IActionResult ChoisirCommande(string CommandeType, int ZoneId, string ConsoType)
    {
        return RedirectToAction("FormulaireCommande", new 
        { 
            CommandeType = CommandeType,
            ZoneId = ZoneId,
            ConsoType = ConsoType 
        });
    }

    [HttpPost]
    public IActionResult TraiterSelection(CommandeViewModel viewModel, string action)
    {
        Console.WriteLine("=== TRAITER SELECTION ===");

        TempData.Remove("SelectedBurgers");
        TempData.Remove("SelectedMenus");
        TempData.Remove("CommandeType");
        TempData.Remove("ZoneId");
        TempData.Remove("ConsoType");

        Console.WriteLine($"CommandeType: {viewModel.CommandeType}");
        
        Console.WriteLine("Données burgers reçues:");
        if (viewModel.BurgerSelections != null)
        {
            foreach (var kvp in viewModel.BurgerSelections)
            {
                var burgerId = kvp.Key;
                var selection = kvp.Value;
                Console.WriteLine($"  Burger {burgerId}: Selected={selection?.IsSelected}, Qte={selection?.Quantite}, Prix={selection?.Prix}");
            }
        }

        var selectedBurgers = new List<BurgerSelection>();
        if (viewModel.BurgerSelections != null)
        {
            foreach (var kvp in viewModel.BurgerSelections)
            {
                var selection = kvp.Value;
                if (selection != null && selection.IsSelected && selection.Quantite > 0)
                {
                    selectedBurgers.Add(selection);
                }
            }
        }

        var selectedMenus = new List<MenuSelection>();
        if (viewModel.MenuSelections != null)
        {
            foreach (var kvp in viewModel.MenuSelections)
            {
                var selection = kvp.Value;
                if (selection != null && selection.IsSelected && selection.Quantite > 0)
                {
                    selectedMenus.Add(selection);
                }
            }
        }

        if (selectedBurgers.Count == 0 && selectedMenus.Count == 0)
        {
            TempData["Error"] = "Veuillez sélectionner au moins un produit.";
            return RedirectToAction("FormulaireCommande", new 
            { 
                CommandeType = viewModel.CommandeType,
                ZoneId = viewModel.ZoneId,
                ConsoType = viewModel.ConsoType 
            });
        }

        TempData["CommandeType"] = viewModel.CommandeType;
        TempData["ZoneId"] = viewModel.ZoneId.ToString();
        TempData["ConsoType"] = viewModel.ConsoType;
        
        if (selectedBurgers.Any())
        {
            TempData["SelectedBurgers"] = JsonSerializer.Serialize(selectedBurgers);
            Console.WriteLine($"Burgers sauvegardés: {selectedBurgers.Count}");
        }
        
        if (selectedMenus.Any())
        {
            TempData["SelectedMenus"] = JsonSerializer.Serialize(selectedMenus);
        }

        if (action == "continuer")
        {
            return RedirectToAction("SelectionComplements");
        }
        else
        {
            return RedirectToAction("FinaliserSansComplements");
        }
    }

    [HttpGet]
    public async Task<IActionResult> FinaliserSansComplements()
    {
        Console.WriteLine("=== FINALISER SANS COMPLEMENTS ===");
        
        return await TraiterFinalisation(new List<ComplementSelection>());
    }

    [HttpPost]
    public async Task<IActionResult> FinaliserAvecComplements(CommandeViewModel viewModel)
    {
        Console.WriteLine("=== FINALISER AVEC COMPLEMENTS ===");

        var selectedComplements = viewModel.ComplementSelections?
            .Where(c => c.IsSelected && c.Quantite > 0)
            .ToList() ?? new List<ComplementSelection>();
        
        Console.WriteLine($"Compléments sélectionnés: {selectedComplements.Count}");
        
        return await TraiterFinalisation(selectedComplements);
    }

    private async Task<IActionResult> TraiterFinalisation(List<ComplementSelection> selectedComplements)
    {
        var commandeType = TempData["CommandeType"] as string;
        var zoneIdStr = TempData["ZoneId"] as string;
        var consoType = TempData["ConsoType"] as string;
        var burgersJson = TempData["SelectedBurgers"] as string;
        var menusJson = TempData["SelectedMenus"] as string;
        
        if (string.IsNullOrEmpty(commandeType) || string.IsNullOrEmpty(zoneIdStr))
        {
            TempData["Error"] = "Session expirée. Veuillez recommencer.";
            return RedirectToAction("PasserCommande");
        }

        var selectedBurgers = !string.IsNullOrEmpty(burgersJson)
            ? JsonSerializer.Deserialize<List<BurgerSelection>>(burgersJson)
            : new List<BurgerSelection>();
            
        var selectedMenus = !string.IsNullOrEmpty(menusJson)
            ? JsonSerializer.Deserialize<List<MenuSelection>>(menusJson)
            : new List<MenuSelection>();

        return await EnregistrerCommandeComplete(
            commandeType,
            int.Parse(zoneIdStr),
            consoType,
            selectedBurgers,
            selectedMenus,
            selectedComplements
        );
    }

    private async Task<IActionResult> EnregistrerCommandeComplete(
        string commandeType,
        int zoneId,
        string consoType,
        List<BurgerSelection> selectedBurgers,
        List<MenuSelection> selectedMenus,
        List<ComplementSelection> selectedComplements)
    {
        try
        {
            Console.WriteLine("=== ENREGISTRER COMMANDE ===");
            Console.WriteLine($"Type: {commandeType}, Burgers: {selectedBurgers.Count}, Menus: {selectedMenus.Count}");

            if (commandeType == "Burger" && selectedMenus.Count > 0)
            {
                Console.WriteLine("⚠️ ERREUR: Commande de type Burger mais menus présents!");
                selectedMenus.Clear(); // Nettoyer les menus
            }
            
            if (commandeType == "Menu" && selectedBurgers.Count > 0)
            {
                Console.WriteLine("⚠️ ERREUR: Commande de type Menu mais burgers présents!");
                selectedBurgers.Clear(); // Nettoyer les burgers
            }
            
            int clientId = int.Parse(User.FindFirst("ClientId").Value);
            
            int nextId = (_context.Commandes.Max(c => (int?)c.Id) ?? 0) + 1;
            string code;
            do
            {
                code = $"CMD-{nextId:D4}";
                nextId++;
            } while (_context.Commandes.Any(c => c.Code == code));

            decimal prixTotal = 0;
         
            var commande = new Commande
            {
                ClientId = clientId,
                ZoneId = zoneId,
                ConsoType = Enum.Parse<ConsoType>(consoType),
                CommandeType = Enum.Parse<CommandeType>(commandeType),
                EtatCommande = EtatCommande.EnAttente,
                Date = DateTime.UtcNow,
                Code = code,
                Prix = 0
            };

            foreach (var burgerSel in selectedBurgers)
            {
                Console.WriteLine($"  - Burger {burgerSel.BurgerId}: {burgerSel.Quantite} x {burgerSel.Prix}");
                
                commande.CommandeBurgers.Add(new CommandeBurger
                {
                    BurgerId = burgerSel.BurgerId,
                    Quantite = burgerSel.Quantite
                });
                prixTotal += burgerSel.Prix * burgerSel.Quantite;
            }

            foreach (var menuSel in selectedMenus)
            {
                commande.CommandeMenus.Add(new CommandeMenu
                {
                    MenuId = menuSel.MenuId,
                    Quantite = menuSel.Quantite
                });
                prixTotal += menuSel.Prix * menuSel.Quantite;
            }

             if (selectedComplements.Any())
            {
                Console.WriteLine($"Ajout de {selectedComplements.Count} complément(s):");
                foreach (var complement in selectedComplements)
                {
                    Console.WriteLine($"  - {complement.ComplementNom}: {complement.Quantite} x {complement.Prix} = {complement.Prix * complement.Quantite}");
                    prixTotal += complement.Prix * complement.Quantite;
                }
            }

            commande.Prix = prixTotal;
            Console.WriteLine($"Prix total final: {prixTotal}");

            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Commande enregistrée avec succès !";
            TempData["CommandeId"] = commande.Id;
            
            Console.WriteLine($"Commande créée avec ID: {commande.Id}");
            
            return RedirectToAction("Confirmation", new { id = commande.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERREUR lors de l'enregistrement: {ex.Message}");
            TempData["Error"] = $"Erreur: {ex.Message}";
            return RedirectToAction("PasserCommande");
        }
    }

    [HttpPost]
    public IActionResult TestCommandeSimple(string CommandeType, int ZoneId, string ConsoType, int BurgerId, int Quantite = 1)
    {
        Console.WriteLine("=== TEST COMMANDE SIMPLE ===");
        
        TempData["CommandeType"] = CommandeType;
        TempData["ZoneId"] = ZoneId.ToString();
        TempData["ConsoType"] = ConsoType;
        TempData["TestBurgerId"] = BurgerId.ToString();
        TempData["TestQuantite"] = Quantite.ToString();
        
        return RedirectToAction("TestFinaliser");
    }

    [HttpGet]
    public async Task<IActionResult> TestFinaliser()
    {
        var commandeType = TempData["CommandeType"] as string ?? "Burger";
        var zoneId = int.Parse(TempData["ZoneId"] as string ?? "1");
        var consoType = TempData["ConsoType"] as string ?? "SurPlace";
        var burgerId = int.Parse(TempData["TestBurgerId"] as string ?? "1");
        var quantite = int.Parse(TempData["TestQuantite"] as string ?? "1");
        
        var burger = await _burgerService.GetByIdAsync(burgerId);
        var prixTotal = burger?.Prix * quantite ?? 0;
        
        var commande = new Commande
        {
            ClientId = int.Parse(User.FindFirst("ClientId").Value),
            ZoneId = zoneId,
            ConsoType = Enum.Parse<ConsoType>(consoType),
            CommandeType = Enum.Parse<CommandeType>(commandeType),
            EtatCommande = EtatCommande.EnAttente,
            Date = DateTime.UtcNow,
            Code = $"TEST-{DateTime.Now:yyyyMMdd-HHmmss}",
            Prix = prixTotal,
            CommandeBurgers = new List<CommandeBurger>
            {
                new CommandeBurger
                {
                    BurgerId = burgerId,
                    Quantite = quantite
                }
            }
        };

        _context.Commandes.Add(commande);
        await _context.SaveChangesAsync();

        return RedirectToAction("Confirmation", new { id = commande.Id });
    }
}