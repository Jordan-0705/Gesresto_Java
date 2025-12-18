using Models.Enums;

namespace Models
{
    public class Commande
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Prix { get; set; }
        public DateTime Date { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int? ZoneId { get; set; }
        public Zone? Zone { get; set; }

        public int? LivreurId { get; set; }
        public Livreur? Livreur { get; set; }

        public EtatCommande EtatCommande { get; set; }
        public CommandeType CommandeType { get; set; }
        public ConsoType ConsoType { get; set; }

        public ICollection<CommandeBurger> CommandeBurgers { get; set; } = new List<CommandeBurger>();
        public ICollection<CommandeMenu> CommandeMenus { get; set; } = new List<CommandeMenu>();

        public Paiement? Paiement { get; set; }
    }
}
