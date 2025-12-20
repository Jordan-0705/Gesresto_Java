using Models;
using Models.Enums;

namespace Services
{
    public interface IComplementService
    {
        Task<IEnumerable<Complement>> GetAllAsync();
        Task<Complement?> GetByIdAsync(int id);
        Task<Complement> CreateAsync(Complement complement);
        Task<Complement> UpdateAsync(Complement complement);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Complement>> GetByTypeAsync(ComplementType type);
        Task<IEnumerable<Complement>> GetPagedAsync(int pageNumber, int pageSize, string? search);
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync();
        
        
    }
}
