using Models;

namespace Services
{
    public interface IBurgerService
    {
        Task<IEnumerable<Burger>> GetAllAsync();
        Task<Burger?> GetByIdAsync(int id);
        Task<Burger> CreateAsync(Burger burger);
        Task<Burger> UpdateAsync(Burger burger);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Burger>> GetPagedAsync(int pageNumber, int pageSize,string? search);
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync(); 
    }
}
