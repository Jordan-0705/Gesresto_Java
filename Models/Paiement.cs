using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("paiement")]  
    public class Paiement
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("date")]
        public DateTime Date { get; set; }
        
        [Column("montant")]
        public decimal Montant { get; set; }

        [Column("commande_id")]  
        public int CommandeId { get; set; }
        
        [ForeignKey("CommandeId")]  
        public Commande Commande { get; set; } = null!;  

        [Column("paiementtype")]
        public PaiementType PaiementType { get; set; }
    }
}