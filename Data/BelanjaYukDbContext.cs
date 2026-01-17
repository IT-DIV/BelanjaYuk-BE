using Microsoft.EntityFrameworkCore;
using BelanjaYuk.API.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BelanjaYuk.API.Models
{
    public abstract class BaseAuditableEntity
    {
        public DateTime DateIn { get; set; }
        public string? UserIn { get; set; }
        public DateTime DateUp { get; set; }
        public string? UserUp { get; set; }
        public bool IsActive { get; set; }
    }
}
public class BelanjaYukDbContext : DbContext
{
    public BelanjaYukDbContext(DbContextOptions<BelanjaYukDbContext> options) : base(options)
    {
    }
    public DbSet<MsUser> MsUsers { get; set; }
    public DbSet<MsUserPassword> MsUserPasswords { get; set; }
    public DbSet<MsProduct> MsProducts { get; set; }
    public DbSet<MsUserSeller> MsUserSellers { get; set; }
    public DbSet<LtCategory> LtCategories { get; set; }
    public DbSet<LtGender> LtGenders { get; set; }
    public DbSet<LtPayment> LtPayments { get; set; }
    public DbSet<TrBuyerCart> TrBuyerCarts { get; set; }
    public DbSet<TrBuyerTransaction> TrBuyerTransactions { get; set; }
    public DbSet<TrBuyerTransactionDetail> TrBuyerTransactionDetails { get; set; }
    public DbSet<TrHomeAddress> TrHomeAddresses { get; set; }
    public DbSet<TrProductImages> TrProductImages { get; set; }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseAuditableEntity>();
        string currentUserId = "SYSTEM";
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateIn = DateTime.UtcNow;
                entry.Entity.UserIn = currentUserId;
                entry.Entity.DateUp = DateTime.UtcNow;
                entry.Entity.UserUp = currentUserId;
                entry.Entity.IsActive = true;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.DateUp = DateTime.UtcNow;
                entry.Entity.UserUp = currentUserId;
                entry.Property(p => p.DateIn).IsModified = false;
                entry.Property(p => p.UserIn).IsModified = false;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<MsUser>().ToTable("MsUser");
        modelBuilder.Entity<MsUserPassword>().ToTable("MsUserPassword");
        modelBuilder.Entity<MsUserSeller>().ToTable("MsUserSeller");
        modelBuilder.Entity<MsProduct>().ToTable("MsProduct");
        modelBuilder.Entity<LtCategory>().ToTable("LtCategory");
        modelBuilder.Entity<LtGender>().ToTable("LtGender");
        modelBuilder.Entity<LtPayment>().ToTable("LtPayment");
        modelBuilder.Entity<TrBuyerCart>().ToTable("TrBuyerCart");
        modelBuilder.Entity<TrBuyerTransaction>().ToTable("TrBuyerTransaction");
        modelBuilder.Entity<TrBuyerTransactionDetail>().ToTable("TrBuyerTransactionDetail");
        modelBuilder.Entity<TrHomeAddress>().ToTable("TrHomeAddress");
        modelBuilder.Entity<TrProductImages>().ToTable("TrProductImages");
    }
}