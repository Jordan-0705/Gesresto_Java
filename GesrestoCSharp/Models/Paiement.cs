using Models.Enums;

namespace Models
{
    public class Paiement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Montant { get; set; }

        public int CommandeId { get; set; }
        public Commande Commande { get; set; }

        public PaiementType PaiementType { get; set; }
    }
}
