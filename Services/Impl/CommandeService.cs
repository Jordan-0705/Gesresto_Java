using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Services.Impl
{
    public class CommandeService : ICommandeService
    {
        private readonly GesRestoDbContext _context;

        public CommandeService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Commande>> GetAllAsync()
        {
            var commandes = await _context.Commandes!
                .Include(c => c.Client)
                .Include(c => c.Livreur)
                .Include(c => c.Zone)
                .Include(c => c.Paiement)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            var commandeIds = commandes.Select(c => c.Id).ToList();

            await _context.CommandeBurgers!
                .Where(cb => commandeIds.Contains(cb.CommandeId))
                .Include(cb => cb.Burger)
                .Include(cb => cb.ComplementF)
                .Include(cb => cb.ComplementB)
                .LoadAsync();

            await _context.CommandeMenus!
                .Where(cm => commandeIds.Contains(cm.CommandeId))
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.Burger)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementF)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementB)
                .LoadAsync();

            return commandes;
        }

        public async Task<Commande?> GetByIdAsync(int id)
        {
            var commande = await _context.Commandes!
                .Include(c => c.Client)
                .Include(c => c.Livreur)
                .Include(c => c.Zone)
                .Include(c => c.Paiement)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (commande == null)
                return null;

            await _context.CommandeBurgers!
                .Where(cb => cb.CommandeId == id)
                .Include(cb => cb.Burger)
                .Include(cb => cb.ComplementF)
                .Include(cb => cb.ComplementB)
                .LoadAsync();

            await _context.CommandeMenus!
                .Where(cm => cm.CommandeId == id)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.Burger)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementF)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementB)
                .LoadAsync();

            return commande;
        }

        public async Task<Commande> CreateAsync(Commande commande)
        {
            _context.Commandes!.Add(commande);
            await _context.SaveChangesAsync();
            return commande;
        }

        public async Task<Commande> UpdateAsync(Commande commande)
        {
            _context.Commandes!.Update(commande);
            await _context.SaveChangesAsync();
            return commande;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var commande = await _context.Commandes!.FindAsync(id);
            if (commande == null) return false;

            _context.Commandes.Remove(commande);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Commande>> GetByClientAsync(int clientId)
        {
            
            var commandes = await _context.Commandes!
                .Where(c => c.ClientId == clientId)
                .Include(c => c.Client)
                .Include(c => c.Livreur)
                .Include(c => c.Zone)
                .Include(c => c.Paiement)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            var commandeIds = commandes.Select(c => c.Id).ToList();

            await _context.CommandeBurgers!
                .Where(cb => commandeIds.Contains(cb.CommandeId))
                .Include(cb => cb.Burger)
                .Include(cb => cb.ComplementF)
                .Include(cb => cb.ComplementB)
                .LoadAsync();

            await _context.CommandeMenus!
                .Where(cm => commandeIds.Contains(cm.CommandeId))
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.Burger)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementF)
                .Include(cm => cm.Menu)
                    .ThenInclude(m => m.ComplementB)
                .LoadAsync();

            return commandes;
        }

        public async Task<IEnumerable<Commande>> GetByEtatAsync(EtatCommande etat)
        {
            return await _context.Commandes!
                .Include(c => c.Client)
                .Where(c => c.EtatCommande == etat)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<PagedResult<Commande>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Commandes!
                .Include(c => c.Client)
                .Include(c => c.Livreur)
                .Include(c => c.Zone)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Commande>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Commandes!.CountAsync();
        }
    }
}
