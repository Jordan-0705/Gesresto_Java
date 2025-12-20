using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IZoneService
    {
        Task<IEnumerable<Zone>> GetAllZonesAsync();
        Task<Zone?> GetByIdAsync(int id);
    }
}
