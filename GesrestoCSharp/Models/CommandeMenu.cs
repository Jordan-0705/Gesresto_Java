namespace Models
{
    public class CommandeMenu
    {
        public int Id { get; set; }

        public int CommandeId { get; set; }
        public Commande Commande { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public int Quantite { get; set; }
    }
}
