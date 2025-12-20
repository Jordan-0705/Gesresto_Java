using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Services.Impl
{
    public class ComplementService : IComplementService
    {
        private readonly GesRestoDbContext _context;

        public ComplementService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Complement>> GetAllAsync()
        {
            return await _context.Complements!
                .Where(c => c.Etat == EtatProduit.Disponible)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Complement?> GetByIdAsync(int id)
        {
            return await _context.Complements!
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Complement> CreateAsync(Complement complement)
        {
            _context.Complements!.Add(complement);
            await _context.SaveChangesAsync();
            return complement;
        }

        public async Task<Complement> UpdateAsync(Complement complement)
        {
            _context.Complements!.Update(complement);
            await _context.SaveChangesAsync();
            return complement;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var complement = await _context.Complements!.FindAsync(id);
            if (complement == null) return false;

            _context.Complements.Remove(complement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Complement>> GetByTypeAsync(ComplementType type)
        {
            return await _context.Complements!
                .Where(c => c.ComplementType == type && c.Etat == EtatProduit.Disponible)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Complement>> GetPagedAsync(
            int page,
            int pageSize,
            string? search = null)
        {
            var query = _context.Complements!
                .Where(c => c.Etat == EtatProduit.Disponible);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    EF.Functions.ILike(c.Nom, $"%{search}%"));
            }

            return await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Complements!.CountAsync();
        }

        public async Task<int> GetAvailableCountAsync()
        {
            return await _context.Complements!
                .Where(c => c.Etat == EtatProduit.Disponible)
                .CountAsync();
        }

        

    }
}
