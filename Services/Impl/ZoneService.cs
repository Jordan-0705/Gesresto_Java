using Models;
using Data; 
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Impl
{
    public class ZoneService : IZoneService
    {
        private readonly GesRestoDbContext _context;

        public ZoneService(GesRestoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            return await _context.Zones.ToListAsync();
        }

        public async Task<Zone?> GetByIdAsync(int id)
        {
            return await _context.Zones.FirstOrDefaultAsync(z => z.Id == id);
        }
    }
}
