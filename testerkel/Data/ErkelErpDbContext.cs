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

        public DbSet<UnitAlias> UnitAliases => Set<UnitAlias>();

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
            });

            // Warehouse
            modelBuilder.Entity<Warehouse>(e =>
            {
                e.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => x.Code)
                    .IsUnique();

                e.Property(x => x.Name)
                    .HasMaxLength(200);

                e.Property(x => x.IsActive)
                    .HasDefaultValue(true);
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

                e.Property(x => x.UnitPrice)
                    .HasPrecision(18, 2);

                e.Property(x => x.LineTotal)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<UnitAlias>(e =>
            {
                e.Property(x => x.Alias)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => x.Alias)
                    .IsUnique();

                e.Property(x => x.UnitType)
                    .HasConversion<byte>();      // UnitType -> tinyint
            });

            modelBuilder.Entity<UnitAlias>().HasData(
                new UnitAlias { Id = 1, UnitType = UnitType.Adet, Alias = "Ad" },
                new UnitAlias { Id = 2, UnitType = UnitType.Adet, Alias = "Ad." },
                new UnitAlias { Id = 3, UnitType = UnitType.Adet, Alias = "Adet" },
                new UnitAlias { Id = 4, UnitType = UnitType.AdTl, Alias = "Ad/Tl" },
                new UnitAlias { Id = 5, UnitType = UnitType.Kilogram, Alias = "Kg" },
                new UnitAlias { Id = 6, UnitType = UnitType.Kilogram, Alias = "Kg." },
                new UnitAlias { Id = 7, UnitType = UnitType.Kilogram, Alias = "Kilogram" },
                new UnitAlias { Id = 8, UnitType = UnitType.Litre, Alias = "Lt" },
                new UnitAlias { Id = 9, UnitType = UnitType.Litre, Alias = "L" },
                new UnitAlias { Id = 10, UnitType = UnitType.Litre, Alias = "Litre" },
                new UnitAlias { Id = 11, UnitType = UnitType.Kutu, Alias = "Kutu" },
                new UnitAlias { Id = 12, UnitType = UnitType.Metre, Alias = "m" },
                new UnitAlias { Id = 13, UnitType = UnitType.Metre, Alias = "m." },
                new UnitAlias { Id = 16, UnitType = UnitType.Metre, Alias = "Mt" },
                new UnitAlias { Id = 17, UnitType = UnitType.Metre, Alias = "Mt." },
                new UnitAlias { Id = 18, UnitType = UnitType.Metre, Alias = "Metre" },
                new UnitAlias { Id = 19, UnitType = UnitType.MetreKare, Alias = "m^2" },
                new UnitAlias { Id = 20, UnitType = UnitType.MetreKare, Alias = "m2" },
                new UnitAlias { Id = 23, UnitType = UnitType.MetreKup, Alias = "m3" },
                new UnitAlias { Id = 26, UnitType = UnitType.MetreKup, Alias = "m^3" },
                new UnitAlias { Id = 27, UnitType = UnitType.Paket, Alias = "Paket" },
                new UnitAlias { Id = 28, UnitType = UnitType.Takım, Alias = "Takım" },
                new UnitAlias { Id = 29, UnitType = UnitType.Takım, Alias = "Tk" },
                new UnitAlias { Id = 30, UnitType = UnitType.Takım, Alias = "Tk." },
                new UnitAlias { Id = 31, UnitType = UnitType.Ton, Alias = "Ton" },
                new UnitAlias { Id = 32, UnitType = UnitType.Ton, Alias = "T" },
                new UnitAlias { Id = 33, UnitType = UnitType.Top, Alias = "Top" },
                new UnitAlias { Id = 34, UnitType = UnitType.Saat, Alias = "Saat" },
                new UnitAlias { Id = 35, UnitType = UnitType.Saat, Alias = "Sa" },
                new UnitAlias { Id = 36, UnitType = UnitType.Saat, Alias = "Sa." },
                new UnitAlias { Id = 37, UnitType = UnitType.Saat, Alias = "St" },
                new UnitAlias { Id = 38, UnitType = UnitType.Cift, Alias = "Çift" },
                new UnitAlias { Id = 39, UnitType = UnitType.Cift, Alias = "Cift" });

        }
    }
}
