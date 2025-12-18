using Models.Enums;

namespace Models
{
    public class Burger
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string photo { get; set; }
        public string Nom { get; set; }
        public decimal Prix { get; set; }
        public EtatProduit Etat { get; set; }

        public ICollection<CommandeBurger> CommandeBurgers { get; set; } = new List<CommandeBurger>();
    }
}
