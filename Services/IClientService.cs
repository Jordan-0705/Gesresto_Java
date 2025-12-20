using Models;

namespace Services
{
    public interface IClientService
    {
        Task<Client?> AuthenticateAsync(string login, string password);
        Task<bool> ExistsLoginAsync(string login);
        Task<Client> CreateAsync(Client client);
        int GetNextId();
        Task<Client> GetByIdAsync(int id);
    }
}
