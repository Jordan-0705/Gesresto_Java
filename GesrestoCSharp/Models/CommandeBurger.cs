namespace Models
{
    public class CommandeBurger
    {
        public int Id { get; set; }

        public int CommandeId { get; set; }
        public Commande Commande { get; set; }

        public int BurgerId { get; set; }
        public Burger Burger { get; set; }

        public int? ComplementFId { get; set; }
        public Complement? ComplementF { get; set; }

        public int? ComplementBId { get; set; }
        public Complement? ComplementB { get; set; }

        public int Quantite { get; set; }
    }
}
