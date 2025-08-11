using Microsoft.EntityFrameworkCore;
using ChronoVoid.API.Models;

namespace ChronoVoid.API.Data;

public class ChronoVoidContext : DbContext
{
    public ChronoVoidContext(DbContextOptions<ChronoVoidContext> options) : base(options) { }
    
    public DbSet<NexusRealm> NexusRealms { get; set; }
    public DbSet<NeuralNode> NeuralNodes { get; set; }
    public DbSet<HyperTunnel> HyperTunnels { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Planet> Planets { get; set; }
    public DbSet<Starbase> Starbases { get; set; }
    public DbSet<AlienRace> AlienRaces { get; set; }

    // Phase 1 additions
    public DbSet<Ship> Ships { get; set; }
    public DbSet<ShipCargo> ShipCargos { get; set; }
    public DbSet<TradeGood> TradeGoods { get; set; }
    public DbSet<TradeTransaction> TradeTransactions { get; set; }
    public DbSet<PlanetProduction> PlanetProductions { get; set; }
    public DbSet<PlanetContract> PlanetContracts { get; set; }
    public DbSet<OwnershipLog> OwnershipLogs { get; set; }
    public DbSet<Troop> Troops { get; set; }
    public DbSet<Faction> Factions { get; set; }
    public DbSet<FactionMember> FactionMembers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // NexusRealm configuration
        modelBuilder.Entity<NexusRealm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });
        
        // NeuralNode configuration
        modelBuilder.Entity<NeuralNode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Realm)
                  .WithMany(r => r.Nodes)
                  .HasForeignKey(e => e.RealmId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.RealmId, e.NodeNumber }).IsUnique();
        });
        
        // HyperTunnel configuration
        modelBuilder.Entity<HyperTunnel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.FromNode)
                  .WithMany(n => n.OutgoingTunnels)
                  .HasForeignKey(e => e.FromNodeId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ToNode)
                  .WithMany(n => n.IncomingTunnels)
                  .HasForeignKey(e => e.ToNodeId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.FromNodeId, e.ToNodeId }).IsUnique();
        });
        
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasOne(e => e.CurrentNode)
                  .WithMany(n => n.CurrentUsers)
                  .HasForeignKey(e => e.CurrentNodeId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Realm)
                  .WithMany(r => r.Users)
                  .HasForeignKey(e => e.RealmId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Planet configuration
        modelBuilder.Entity<Planet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Node)
                  .WithMany(n => n.Planets)
                  .HasForeignKey(e => e.NodeId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Owner)
                  .WithMany(u => u.OwnedPlanets)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.NodeId, e.PlanetNumber }).IsUnique();
        });
        
        // Starbase configuration
        modelBuilder.Entity<Starbase>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Node)
                  .WithOne(n => n.Starbase)
                  .HasForeignKey<Starbase>(e => e.NodeId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Owner)
                  .WithMany(u => u.OwnedStarbases)
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Ship configuration
        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Owner)
                  .WithMany()
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.CurrentNode)
                  .WithMany()
                  .HasForeignKey(e => e.CurrentNodeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ShipCargo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Ship)
                  .WithMany(s => s.Cargo)
                  .HasForeignKey(e => e.ShipId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Trade/Economy configuration
        modelBuilder.Entity<TradeGood>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResourceType).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Starbase)
                  .WithMany()
                  .HasForeignKey(e => e.StarbaseId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.StarbaseId, e.ResourceType }).IsUnique();
        });

        modelBuilder.Entity<TradeTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Starbase)
                  .WithMany()
                  .HasForeignKey(e => e.StarbaseId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Buyer)
                  .WithMany()
                  .HasForeignKey(e => e.BuyerId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Seller)
                  .WithMany()
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlanetProduction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Planet)
                  .WithMany()
                  .HasForeignKey(e => e.PlanetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlanetContract>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Planet)
                  .WithMany()
                  .HasForeignKey(e => e.PlanetId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Starbase)
                  .WithMany()
                  .HasForeignKey(e => e.StarbaseId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OwnershipLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Planet)
                  .WithMany()
                  .HasForeignKey(e => e.PlanetId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PreviousOwner)
                  .WithMany()
                  .HasForeignKey(e => e.PreviousOwnerId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.NewOwner)
                  .WithMany()
                  .HasForeignKey(e => e.NewOwnerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Troop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Owner)
                  .WithMany()
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Planet)
                  .WithMany()
                  .HasForeignKey(e => e.PlanetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Faction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<FactionMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Faction)
                  .WithMany(f => f.Members)
                  .HasForeignKey(e => e.FactionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.FactionId, e.UserId }).IsUnique();
        });
    }
}
