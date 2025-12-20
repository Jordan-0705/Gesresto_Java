// Controllers/MesCommandesController.cs
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;
using Npgsql;
using Services;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class MesCommandesController : Controller
{
    private readonly GesRestoDbContext _context;
    private readonly IClientService _clientService;

    public MesCommandesController(GesRestoDbContext context, IClientService clientService)
    {
        _context = context;
        _clientService = clientService;
    }
    
    public async Task<IActionResult> Index()
    {
        int clientId = int.Parse(User.FindFirst("ClientId").Value);
        
        var client = await _clientService.GetByIdAsync(clientId);
        ViewBag.UserName = client?.Nom ?? "Client";
  
        var commandes = await _context.Commandes
            .Where(c => c.ClientId == clientId)
            .Include(c => c.Zone)  
            .OrderByDescending(c => c.Date)
            .ToListAsync();

        return View(commandes);
    }

    [HttpGet]
    public async Task<IActionResult> Payer(int id)
    {
        int clientId = int.Parse(User.FindFirst("ClientId").Value);
        
        var commande = await _context.Commandes
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id && c.ClientId == clientId);
        
        if (commande == null)
        {
            return NotFound();
        }

        if (commande.EtatCommande != EtatCommande.EnAttente)
        {
            TempData["Error"] = "Cette commande ne peut pas être payée.";
            return RedirectToAction("Index");
        }

        ViewBag.UserName = commande.Client?.Nom ?? "Client";
        return View(commande);
    }

    
    [HttpPost]
    public async Task<IActionResult> ProcesserPaiement(int commandeId, string paiementType)
    {
        int clientId = int.Parse(User.FindFirst("ClientId").Value);
        
        var commande = await _context.Commandes
            .FirstOrDefaultAsync(c => c.Id == commandeId && c.ClientId == clientId);
        
        if (commande == null)
        {
            TempData["Error"] = "Commande non trouvée.";
            return RedirectToAction("Index");
        }

        // MÉTHODE 1 : Vérifiez l'état avec une requête SQL directe simple
        var connection = _context.Database.GetDbConnection();
        
        try
        {
            await connection.OpenAsync();
            
            // Vérifiez l'état
            using var commandCheck = connection.CreateCommand();
            commandCheck.CommandText = "SELECT etatcommande FROM commande WHERE id = @id";
            commandCheck.Parameters.Add(new NpgsqlParameter("@id", commandeId));
            
            var etat = await commandCheck.ExecuteScalarAsync() as string;
            
            if (etat != "En Attente")
            {
                TempData["Error"] = "Cette commande ne peut pas être payée.";
                return RedirectToAction("Index");
            }

            // Transaction pour mettre à jour la commande et créer le paiement
            using var transaction = await connection.BeginTransactionAsync();
            
            try
            {
                
                using var cmdUpdate = connection.CreateCommand();
                cmdUpdate.Transaction = transaction;
                cmdUpdate.CommandText = "UPDATE commande SET etatcommande = 'Valide' WHERE id = @id";
                cmdUpdate.Parameters.Add(new NpgsqlParameter("@id", commandeId));
                await cmdUpdate.ExecuteNonQueryAsync();

                
                using var cmdInsert = connection.CreateCommand();
                cmdInsert.Transaction = transaction;
                cmdInsert.CommandText = @"
                    INSERT INTO paiement (commande_id, montant, date, paiementtype) 
                    VALUES (@commande_id, @montant, @date, @paiementtype)";
                
                cmdInsert.Parameters.Add(new NpgsqlParameter("@commande_id", commandeId));
                cmdInsert.Parameters.Add(new NpgsqlParameter("@montant", commande.Prix));
                cmdInsert.Parameters.Add(new NpgsqlParameter("@date", DateTime.UtcNow));
                cmdInsert.Parameters.Add(new NpgsqlParameter("@paiementtype", paiementType));
                
                await cmdInsert.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                
                TempData["Message"] = $"✅ Paiement de {commande.Prix.ToString("N0")} FCFA effectué avec succès via {paiementType} !";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"❌ Erreur lors du paiement: {ex.Message}";
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"❌ Erreur de connexion: {ex.Message}";
        }

        return RedirectToAction("Index");
    }

   
    [HttpPost]
    public async Task<IActionResult> Annuler(int id)
    {
        int clientId = int.Parse(User.FindFirst("ClientId").Value);
        
        var commande = await _context.Commandes
            .FirstOrDefaultAsync(c => c.Id == id && c.ClientId == clientId);
        
        if (commande == null)
        {
            return NotFound();
        }

      
        commande.EtatCommande = EtatCommande.Annule;
        await _context.SaveChangesAsync();

        TempData["Message"] = "Commande annulée avec succès.";
        return RedirectToAction("Index");
    }

  
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        int clientId = int.Parse(User.FindFirst("ClientId").Value);
        
        var commande = await _context.Commandes
            .Include(c => c.Client)
            .Include(c => c.Zone)
            .Include(c => c.Paiement)
            .FirstOrDefaultAsync(c => c.Id == id && c.ClientId == clientId);
        
        if (commande == null) return NotFound();
        
        // Chargez les produits uniquement pour cette page
        await _context.Entry(commande)
            .Collection(c => c.CommandeBurgers)
            .Query()
            .Include(cb => cb.Burger)
            .LoadAsync();
            
        await _context.Entry(commande)
            .Collection(c => c.CommandeMenus)
            .Query()
            .Include(cm => cm.Menu)
            .ThenInclude(m => m.Burger)
            .LoadAsync();

        ViewBag.UserName = commande.Client?.Nom ?? "Client";
        return View(commande);
    }
}