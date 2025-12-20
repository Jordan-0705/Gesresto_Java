using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Services.Impl
{
    public class MenuService : IMenuService
    {
        private readonly GesRestoDbContext _context;

        public MenuService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus!
                .Include(m => m.Burger)
                .Include(m => m.ComplementF)
                .Include(m => m.ComplementB)
                .Where(m => m.Etat == Models.Enums.EtatProduit.Disponible)
                .OrderBy(m => m.Id)
                .ToListAsync();
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus!
                .Include(m => m.Burger)
                .Include(m => m.ComplementF)
                .Include(m => m.ComplementB)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Menu> CreateAsync(Menu menu)
        {
            _context.Menus!.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<Menu> UpdateAsync(Menu menu)
        {
            _context.Menus!.Update(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menu = await _context.Menus!.FindAsync(id);
            if (menu == null) return false;

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Menu>> GetPagedAsync(
            int page,
            int pageSize,
            string? search = null)
        {
            var query = _context.Menus!
                .Include(m => m.Burger)
                .Include(m => m.ComplementF)
                .Include(m => m.ComplementB)
                .Where(m => m.Etat == EtatProduit.Disponible);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    EF.Functions.ILike(m.Nom, $"%{search}%"));
            }

            return await query
                .OrderBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Menus!.CountAsync();
        }

        public async Task<int> GetAvailableCountAsync()
        {
            return await _context.Menus!.Where(m => m.Etat == Models.Enums.EtatProduit.Disponible).CountAsync();
        }
    }
}
