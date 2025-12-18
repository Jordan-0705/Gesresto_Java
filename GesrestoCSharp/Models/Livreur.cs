namespace Models
{
    public class Livreur
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Nom { get; set; }
        public string? Telephone { get; set; }

        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    }
}
