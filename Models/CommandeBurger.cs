using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("commandeburger")]
    public class CommandeBurger
    {
        public int Id { get; set; }  
        
        public int CommandeId { get; set; }  
        
        public int BurgerId { get; set; }  
        
        public int? ComplementFId { get; set; }  
        
        public int? ComplementBId { get; set; }  
        
        public int Quantite { get; set; } = 1; 

        public virtual Commande? Commande { get; set; }
        public virtual Burger? Burger { get; set; }
        public virtual Complement? ComplementF { get; set; }
        public virtual Complement? ComplementB { get; set; }
    }
}