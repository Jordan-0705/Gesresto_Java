using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Services.Impl
{
    public class BurgerService : IBurgerService
    {
        private readonly GesRestoDbContext _context;

        public BurgerService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Burger>> GetAllAsync()
        {
            return await _context.Burgers!
                .Where(b => b.Etat == EtatProduit.Disponible)
                .OrderBy(b => b.Id)
                .Select(b => new Burger
                {
                    Id = b.Id,
                    Code = b.Code,
                    Nom = b.Nom,
                    Prix = b.Prix,
                    Etat = b.Etat,
                    Photo = b.Photo ?? "" 
                })
                .ToListAsync();
        }


        public async Task<Burger?> GetByIdAsync(int id)
        {
            var b = await _context.Burgers!
                .FirstOrDefaultAsync(b => b.Id == id);

            if (b == null) return null;

            b.Photo ??= ""; 
            return b;
        }

       
        public async Task<Burger> CreateAsync(Burger burger)
        {
            _context.Burgers!.Add(burger);
            await _context.SaveChangesAsync();
            return burger;
        }

        
        public async Task<Burger> UpdateAsync(Burger burger)
        {
            _context.Burgers!.Update(burger);
            await _context.SaveChangesAsync();
            return burger;
        }

        
        public async Task<bool> DeleteAsync(int id)
        {
            var burger = await _context.Burgers!.FindAsync(id);
            if (burger == null) return false;

            _context.Burgers.Remove(burger);
            await _context.SaveChangesAsync();
            return true;
        }

       
        public async Task<IEnumerable<Burger>> GetPagedAsync(
            int page,
            int pageSize,
            string? search = null)
        {
            var query = _context.Burgers!
                .Where(b => b.Etat == EtatProduit.Disponible);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    EF.Functions.ILike(b.Nom, $"%{search}%"));
            }

            return await query
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Burgers!.CountAsync();
        }

        
        public async Task<int> GetAvailableCountAsync()
        {
            return await _context.Burgers!
                .Where(b => b.Etat == EtatProduit.Disponible)
                .CountAsync();
        }
    }
}
