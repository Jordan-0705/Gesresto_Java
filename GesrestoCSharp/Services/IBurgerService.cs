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

        Task<PagedResult<Burger>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync(); // Pour filtrer les burgers disponibles
    }
}
