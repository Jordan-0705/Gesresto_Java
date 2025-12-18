using Models.Enums;

namespace Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string photo { get; set; }
        public string Nom { get; set; }

        public int BurgerId { get; set; }
        public Burger Burger { get; set; }

        public int? ComplementFId { get; set; }
        public Complement? ComplementF { get; set; }

        public int? ComplementBId { get; set; }
        public Complement? ComplementB { get; set; }

        public decimal Prix { get; set; }
        public EtatProduit Etat { get; set; }

        public ICollection<CommandeMenu> CommandeMenus { get; set; } = new List<CommandeMenu>();
    }
}
