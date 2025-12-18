using Models;

namespace Services
{
    public interface IPaiementService
    {
        Task<Paiement> CreateAsync(Paiement paiement);
        Task<Paiement?> GetByIdAsync(int id);
        Task<IEnumerable<Paiement>> GetByCommandeIdAsync(int commandeId);
    }
}
