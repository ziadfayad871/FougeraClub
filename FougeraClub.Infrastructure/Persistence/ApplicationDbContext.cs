using Microsoft.EntityFrameworkCore;
using FougeraClub.Domain.Entities;

namespace FougeraClub.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure PurchaseOrder -> PurchaseOrderItem relationship
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.Items)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Supplier -> PurchaseOrder relationship
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.PurchaseOrders)
                .WithOne(po => po.Supplier)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid accidental manager deletion deleting orders

            // Precision for decimals
            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.Quantity)
                .HasPrecision(18, 2);
        }
    }
}
