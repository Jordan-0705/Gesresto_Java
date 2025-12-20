using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("commande")]
    public class Commande
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public string Code { get; set; } = "";

        [Column("prix")]
        public decimal Prix { get; set; }

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Column("client_id")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Column("zone_id")]
        public int? ZoneId { get; set; }

        [ForeignKey(nameof(ZoneId))]
        public Zone? Zone { get; set; }

        [Column("livreur_id")]
        public int? LivreurId { get; set; }

        [ForeignKey(nameof(LivreurId))]
        public Livreur? Livreur { get; set; }

        [Column("etatcommande")]
        public EtatCommande EtatCommande { get; set; }

        [Column("commandetype")]
        public CommandeType CommandeType { get; set; }

        [Column("consotype")]
        public ConsoType ConsoType { get; set; }

        // ✅ Collections pour la navigation
        public ICollection<CommandeBurger> CommandeBurgers { get; set; } = new List<CommandeBurger>();
        public ICollection<CommandeMenu> CommandeMenus { get; set; } = new List<CommandeMenu>();

        public Paiement? Paiement { get; set; }
    }
}
