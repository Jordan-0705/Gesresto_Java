using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models;
using Models.Enums;

namespace Data
{
    public class GesRestoDbContext : DbContext
    {
        public GesRestoDbContext(DbContextOptions<GesRestoDbContext> options) : base(options) { }
        public DbSet<Client>? Clients { get; set; }
        public DbSet<Livreur>? Livreurs { get; set; }
        public DbSet<Zone>? Zones { get; set; }
        public DbSet<Burger>? Burgers { get; set; }
        public DbSet<Menu>? Menus { get; set; }
        public DbSet<Complement>? Complements { get; set; }
        public DbSet<Commande>? Commandes { get; set; }
        public DbSet<CommandeBurger>? CommandeBurgers { get; set; }
        public DbSet<CommandeMenu>? CommandeMenus { get; set; }
        public DbSet<Paiement>? Paiements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // CLIENT
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50).HasColumnName("code");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
                entity.Property(e => e.Telephone).HasMaxLength(20).HasColumnName("telephone");
                entity.Property(e => e.Login).IsRequired().HasMaxLength(50).HasColumnName("login");
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100).HasColumnName("password");
            });

            // LIVREUR
            modelBuilder.Entity<Livreur>(entity =>
            {
                entity.ToTable("livreur");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50).HasColumnName("code");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
                entity.Property(e => e.Telephone).HasMaxLength(20).HasColumnName("telephone");
            });

            // ZONE
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("zone");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
            });

            
            // BURGER 
            modelBuilder.Entity<Burger>(entity =>
            {
                entity.ToTable("burger");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50).HasColumnName("code");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
                entity.Property(e => e.Prix).IsRequired().HasColumnName("prix");
                entity.Property(e => e.Etat).HasConversion<string>().HasColumnName("etat");
                entity.Property(e => e.Photo).HasMaxLength(255).HasDefaultValue("").HasColumnName("photo");
                
            });

            // COMPLEMENT
            modelBuilder.Entity<Complement>(entity =>
            {
                entity.ToTable("complement");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50).HasColumnName("code");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
                entity.Property(e => e.Prix).IsRequired().HasColumnName("prix");
                entity.Property(e => e.ComplementType).HasConversion<string>().HasColumnName("complement_type");
                entity.Property(e => e.Etat).HasConversion<string>().HasColumnName("etat");
                entity.Property(e => e.Photo).HasMaxLength(255).HasDefaultValue("").HasColumnName("photo");
            });

            // MENU
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("menu");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50).HasColumnName("code");
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(100).HasColumnName("nom");
                entity.Property(e => e.Prix).IsRequired().HasColumnName("prix");
                entity.Property(e => e.Etat).HasConversion<string>().HasColumnName("etat");
                entity.Property(e => e.Photo).HasMaxLength(255).HasDefaultValue("").HasColumnName("photo");

                entity.Property(e => e.BurgerId).HasColumnName("burger_id"); // <--- important
                entity.Property(e => e.ComplementFId).HasColumnName("frites_id");
                entity.Property(e => e.ComplementBId).HasColumnName("boisson_id");

                entity.HasOne(m => m.Burger)
                      .WithMany()
                      .HasForeignKey(m => m.BurgerId)
                      .HasConstraintName("fk_menu_burger")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.ComplementF)
                      .WithMany()
                      .HasForeignKey(m => m.ComplementFId)
                      .HasConstraintName("fk_menu_complementf");

                entity.HasOne(m => m.ComplementB)
                      .WithMany()
                      .HasForeignKey(m => m.ComplementBId)
                      .HasConstraintName("fk_menu_complementb");
            });

            modelBuilder.Entity<Commande>(entity =>
            {
                entity.ToTable("commande");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasColumnName("code");
                entity.Property(e => e.Prix).HasColumnName("prix");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.ClientId).HasColumnName("client_id");
                entity.Property(e => e.ZoneId).HasColumnName("zone_id");
                entity.Property(e => e.LivreurId).HasColumnName("livreur_id");
                
                entity.Property(e => e.EtatCommande)
                    .HasConversion(
                        v => v.ToString().Replace("EnAttente", "En Attente"),
                        v => v == "En Attente" ? EtatCommande.EnAttente : 
                            v == "Valide" ? EtatCommande.Valide :
                            v == "Termine" ? EtatCommande.Termine :
                            EtatCommande.Annule 
                    )
                    .HasColumnName("etatcommande");
                
                
                entity.Property(e => e.EtatCommande)
                    .HasMaxLength(50); 
                
                entity.Property(e => e.CommandeType)
                    .HasConversion<string>()
                    .HasColumnName("commandetype");

                entity.Property(e => e.ConsoType)
                    .HasConversion<string>()
                    .HasColumnName("consotype");

                entity.HasOne(c => c.Client)
                    .WithMany()
                    .HasForeignKey(c => c.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Zone)
                    .WithMany()
                    .HasForeignKey(c => c.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Livreur)
                    .WithMany()
                    .HasForeignKey(c => c.LivreurId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // COMMANDEBURGER
            modelBuilder.Entity<CommandeBurger>(entity =>
            {
                entity.ToTable("commandeburger");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CommandeId).HasColumnName("commande_id");
                entity.Property(e => e.BurgerId).HasColumnName("burger_id");
                entity.Property(e => e.ComplementFId).HasColumnName("complementf_id");
                entity.Property(e => e.ComplementBId).HasColumnName("complementb_id");
                entity.Property(e => e.Quantite).HasColumnName("quantite");

                entity.HasOne(cb => cb.Commande)
                    .WithMany(c => c.CommandeBurgers)
                    .HasForeignKey(cb => cb.CommandeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // COMMANDEMENU
            modelBuilder.Entity<CommandeMenu>(entity =>
            {
                entity.ToTable("commandemenu");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CommandeId).HasColumnName("commande_id");
                entity.Property(e => e.MenuId).HasColumnName("menu_id");
                entity.Property(e => e.Quantite).HasColumnName("quantite");

                entity.HasOne(cm => cm.Commande)
                    .WithMany(c => c.CommandeMenus)
                    .HasForeignKey(cm => cm.CommandeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            
            // PAIEMENT
            modelBuilder.Entity<Paiement>(entity =>
            {
                entity.ToTable("paiement");
                entity.HasKey(e => e.Id);
               
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.Property(e => e.Montant)
                    .HasColumnName("montant")
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.CommandeId)
                    .HasColumnName("commande_id");
                
                entity.Property(e => e.PaiementType)
                    .HasConversion<string>()
                    .HasColumnName("paiementtype")
                    .HasMaxLength(50);

                entity.HasOne(p => p.Commande)
                    .WithOne(c => c.Paiement)
                    .HasForeignKey<Paiement>(p => p.CommandeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=ep-small-rain-ahcxmlt5-pooler.c-3.us-east-1.aws.neon.tech;" +
                    "Database=neondb;" +
                    "Username=neondb_owner;" +
                    "Password=npg_YBwMo1pc5tPd;" +
                    "Ssl Mode=Require;" +
                    "Channel Binding=Require")
            .LogTo(Console.WriteLine, 
                new[] { DbLoggerCategory.Database.Command.Name }, 
                LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
                
            }
        }
    }
}
