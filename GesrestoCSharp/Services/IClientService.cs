using Models;

namespace Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> CreateAsync(Client client);
        Task<Client> UpdateAsync(Client client);
        Task<bool> DeleteAsync(int id);

        Task<Client?> GetByLoginAsync(string login);

        Task<PagedResult<Client>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync();
    }
}
