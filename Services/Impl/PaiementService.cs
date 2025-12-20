using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Impl
{
    public class PaiementService : IPaiementService
    {
        private readonly GesRestoDbContext _context;

        public PaiementService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<Paiement> CreateAsync(Paiement paiement)
        {
            _context.Paiements!.Add(paiement);
            await _context.SaveChangesAsync();
            return paiement;
        }

        public async Task<Paiement?> GetByIdAsync(int id)
        {
            return await _context.Paiements!
                .Include(p => p.Commande)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Paiement>> GetByCommandeIdAsync(int commandeId)
        {
            return await _context.Paiements!
                .Include(p => p.Commande)
                .Where(p => p.CommandeId == commandeId)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Paiement>> GetAllAsync()
        {
            return await _context.Paiements!
                .Include(p => p.Commande)
                .OrderByDescending(p => p.Date)
                .ToListAsync();
        }

        public async Task<Paiement?> UpdateAsync(Paiement paiement)
        {
            var existing = await _context.Paiements!.FindAsync(paiement.Id);
            if (existing == null) return null;

            existing.Montant = paiement.Montant;
            existing.Date = paiement.Date;
            existing.CommandeId = paiement.CommandeId;

            _context.Paiements.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var paiement = await _context.Paiements!.FindAsync(id);
            if (paiement == null) return false;

            _context.Paiements.Remove(paiement);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
