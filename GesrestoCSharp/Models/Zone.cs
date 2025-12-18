namespace Models
{
    public class Zone
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}
