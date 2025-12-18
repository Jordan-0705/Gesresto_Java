using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class GesRestoDbContext : DbContext
    {
        public DbSet<Client>? Clients { get; set; } = null;
        public DbSet<Livreur>? Livreurs { get; set; } = null;
        public DbSet<Zone>? Zones { get; set; } = null;

        public DbSet<Burger>? Burgers { get; set; } = null;
        public DbSet<Menu>? Menus { get; set; } = null;
        public DbSet<Complement>? Complements { get; set; } = null;

        public DbSet<Commande>? Commandes { get; set; } = null;
        public DbSet<CommandeBurger>? CommandeBurgers { get; set; } = null;
        public DbSet<CommandeMenu>? CommandeMenus { get; set; } = null;

        public DbSet<Paiement>? Paiements { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Client
            modelBuilder.Entity<Client>()
                .ToTable("client");
            modelBuilder.Entity<Client>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Client>()
                .Property(c => c.Code).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Client>()
                .Property(c => c.Nom).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Client>()
                .Property(c => c.Telephone).HasMaxLength(20);
            modelBuilder.Entity<Client>()
                .Property(c => c.Login).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Client>()
                .Property(c => c.Password).IsRequired().HasMaxLength(100);

            
            // GESTIONNAIRE
            // modelBuilder.Entity<Gestionnaire>()
            //     .ToTable("gestionnaire");
            // modelBuilder.Entity<Gestionnaire>()
            //     .HasKey(g => g.Id);

            // Livreur
            modelBuilder.Entity<Livreur>()
                .ToTable("livreur");
            modelBuilder.Entity<Livreur>()
                .HasKey(l => l.Id);

            // Zone
            modelBuilder.Entity<Zone>()
                .ToTable("zone");
            modelBuilder.Entity<Zone>()
                .HasKey(z => z.Id);

            // BURGER
            modelBuilder.Entity<Burger>()
                .ToTable("burger");
            modelBuilder.Entity<Burger>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<Burger>()
                .Property(b => b.Nom).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Burger>()
                .Property(b => b.Prix).IsRequired();
            modelBuilder.Entity<Burger>()
                .Property(b => b.Etat).HasConversion<string>();
            modelBuilder.Entity<Burger>()
                .Property(b => b.photo).HasMaxLength(255);

            // COMPLEMENT
            modelBuilder.Entity<Complement>()
                .ToTable("complement");
            modelBuilder.Entity<Complement>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Complement>()
                .Property(c => c.ComplementType).HasConversion<string>();
            modelBuilder.Entity<Complement>()
                .Property(c => c.Etat).HasConversion<string>();
            modelBuilder.Entity<Complement>()
                .Property(c => c.photo).HasMaxLength(255);

            // MENU
            modelBuilder.Entity<Menu>()
                .ToTable("menu");
            modelBuilder.Entity<Menu>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Burger)
                .WithMany()
                .HasForeignKey(m => m.BurgerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Menu>()
                .Property(m => m.photo).HasMaxLength(255);
            modelBuilder.Entity<Menu>()
                .Property(m => m.Etat).HasConversion<string>();

            // Commande
            modelBuilder.Entity<Commande>()
                .ToTable("commande");
            modelBuilder.Entity<Commande>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Commande>()
                .Property(c => c.EtatCommande).HasConversion<string>();
            modelBuilder.Entity<Commande>()
                .Property(c => c.CommandeType).HasConversion<string>();
            modelBuilder.Entity<Commande>()
                .Property(c => c.ConsoType).HasConversion<string>();

            modelBuilder.Entity<Commande>()
                .HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId);

            // Commande Burger
            modelBuilder.Entity<CommandeBurger>()
                .ToTable("commandeburger");
            modelBuilder.Entity<CommandeBurger>()
                .HasKey(cb => cb.Id);

            // Commande Menu
            modelBuilder.Entity<CommandeMenu>()
                .ToTable("commandemenu");
            modelBuilder.Entity<CommandeMenu>()
                .HasKey(cm => cm.Id);

            // Paiement
            modelBuilder.Entity<Paiement>()
                .ToTable("paiement");
            modelBuilder.Entity<Paiement>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Paiement>()
                .Property(p => p.PaiementType).HasConversion<string>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=ep-small-rain-ahcxmlt5-pooler.c-3.us-east-1.aws.neon.tech;" +
                "Database=neondb;" +
                "Username=neondb_owner;" +
                "Password=npg_YBwMo1pc5tPd;" +
                "Ssl Mode=Require;" +
                "Channel Binding=Require"
            );
        }
    }
}
