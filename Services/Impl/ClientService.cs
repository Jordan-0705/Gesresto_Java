using Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Services
{
    public class ClientService : IClientService
    {
        private readonly GesRestoDbContext _context;

        public ClientService(GesRestoDbContext context)
        {
            _context = context;
        }

       
        public async Task<Client?> AuthenticateAsync(string login, string password)
        {
            return await _context.Clients
                                 .FirstOrDefaultAsync(c => c.Login == login && c.Password == password);
        }

      
        public async Task<bool> ExistsLoginAsync(string login)
        {
            return await _context.Clients.AnyAsync(c => c.Login == login);
        }

       
        public async Task<Client> CreateAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }
    

        public int GetNextId()
        {
            var maxId = _context.Clients.Max(c => (int?)c.Id) ?? 0;
            return maxId + 1;
        }

        public async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
