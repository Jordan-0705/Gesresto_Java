using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("commandemenu")]
public class CommandeMenu
{
    [Column("id")]
    public int Id { get; set; }

    [Column("commande_id")]
    public int CommandeId { get; set; }

    [Column("menu_id")]
    public int MenuId { get; set; }

    [Column("quantite")]
    public int Quantite { get; set; } = 1;

    public Commande Commande { get; set; } = null!;
    public Menu Menu { get; set; } = null!;
}

}
