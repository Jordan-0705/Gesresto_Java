using Models;
using Models.Enums;

namespace Services
{
    public interface ICommandeService
    {
        Task<IEnumerable<Commande>> GetAllAsync();
        Task<Commande?> GetByIdAsync(int id);
        Task<Commande> CreateAsync(Commande commande);
        Task<Commande> UpdateAsync(Commande commande);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Commande>> GetByClientAsync(int clientId);
        Task<IEnumerable<Commande>> GetByEtatAsync(EtatCommande etat);

        Task<PagedResult<Commande>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync();
    }
}
