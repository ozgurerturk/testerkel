using Microsoft.EntityFrameworkCore;
using testerkel.Models;
namespace testerkel.Data
{
    public class ErkelErpDbContext : DbContext
    {
        public ErkelErpDbContext(DbContextOptions<ErkelErpDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
        public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();

        public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
        public DbSet<StockTransferLine> StockTransferLines => Set<StockTransferLine>();

        public DbSet<Consumption> Consumptions => Set<Consumption>();
        public DbSet<ConsumptionLine> ConsumptionLines => Set<ConsumptionLine>();

        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();

        public DbSet<StockTxn> StockTxns => Set<StockTxn>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => x.Code)
                    .IsUnique();

                e.Property(x => x.Name)
                    .HasMaxLength(200);

                e.Property(x => x.Unit)
                    .HasConversion<byte>();      // UnitType -> tinyint

                e.Property(x => x.Price)
                    .HasPrecision(18, 2);
            });

            // Warehouse
            modelBuilder.Entity<Warehouse>(e =>
            {
                e.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // Customer
            modelBuilder.Entity<Customer>(e =>
            {
                e.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.TaxNo)
                    .HasMaxLength(20);
            });

            // SalesOrder & Line
            modelBuilder.Entity<SalesOrder>(e =>
            {
                e.Property(x => x.Status)
                    .HasMaxLength(20)
                    .IsRequired();

                e.HasOne(x => x.Customer)
                    .WithMany(c => c.SalesOrders)
                    .HasForeignKey(x => x.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesOrderLine>(e =>
            {
                e.Property(x => x.Qty)
                    .HasPrecision(18, 2);

                e.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);
            });

            // StockTransfer & Line
            modelBuilder.Entity<StockTransfer>(e =>
            {
                e.HasOne(x => x.FromWarehouse)
                    .WithMany()
                    .HasForeignKey(x => x.FromWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.ToWarehouse)
                    .WithMany()
                    .HasForeignKey(x => x.ToWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.Notes)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<StockTransferLine>(e =>
            {
                e.Property(x => x.Qty)
                    .HasPrecision(18, 2);
            });

            // Consumption & Line
            modelBuilder.Entity<Consumption>(e =>
            {
                e.Property(x => x.Notes)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<ConsumptionLine>(e =>
            {
                e.Property(x => x.Qty)
                    .HasPrecision(18, 2);
            });

            // PurchaseOrder & Line
            modelBuilder.Entity<PurchaseOrder>(e =>
            {
                e.Property(x => x.SupplierName)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(x => x.Notes)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<PurchaseOrderLine>(e =>
            {
                e.Property(x => x.Qty)
                    .HasPrecision(18, 2);

                e.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);
            });

            // StockTxn
            modelBuilder.Entity<StockTxn>(e =>
            {
                e.Property(x => x.Qty)
                    .HasPrecision(18, 2);

                e.Property(x => x.Direction)
                    .HasConversion<byte>();          // enum -> tinyint

                e.Property(x => x.MovementType)
                    .HasConversion<byte>();

                e.Property(x => x.RefModule)
                    .HasMaxLength(10);
            });
        }
    }
}
