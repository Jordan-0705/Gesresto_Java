using Models;

namespace Services
{
    public interface IMenuService
    {
        Task<IEnumerable<Menu>> GetAllAsync();
        Task<Menu?> GetByIdAsync(int id);
        Task<Menu> CreateAsync(Menu menu);
        Task<Menu> UpdateAsync(Menu menu);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Menu>> GetPagedAsync(int pageNumber, int pageSize, string? search);
        Task<int> GetTotalCountAsync();
        Task<int> GetAvailableCountAsync();
    }
}
