using Microsoft.EntityFrameworkCore;
using backend.Models.Entities;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Remnant> Remnants { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<CustomerInfo> CustomerInfos { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }

        public DbSet<PriceUpdate> PriceUpdates { get; set; }
        public DbSet<RemnantUpdate> RemnantUpdates { get; set; }
        public DbSet<StockUpdate> StockUpdates { get; set; }
        public DbSet<UpdateLog> UpdateLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.HasKey(e => e.IDType);
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.IDParentType)
                    .HasMaxLength(36)
                    .IsRequired(false);
                entity.Property(e => e.OriginalGuid)
                    .HasMaxLength(36)
                    .IsRequired(false);
            });

            // Nomenclature - УПРОЩЕННАЯ КОНФИГУРАЦИЯ БЕЗ НАВИГАЦИОННЫХ СВОЙСТВ
            modelBuilder.Entity<Nomenclature>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.Gost)
                    .HasMaxLength(200);
                entity.Property(e => e.ProductionType)
                    .HasMaxLength(100);
                entity.Property(e => e.FormOfLength)
                    .HasMaxLength(100);
                entity.Property(e => e.Manufacturer)
                    .HasMaxLength(200);
                entity.Property(e => e.SteelGrade)
                    .HasMaxLength(100);
                entity.Property(e => e.Status)
                    .HasMaxLength(50);
                entity.Property(e => e.Diameter)
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.ProfileSize2)
                    .HasColumnType("decimal(10,2)")
                    .IsRequired(false);
                entity.Property(e => e.PipeWallThickness)
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.Koef)
                    .HasColumnType("decimal(10,6)");

                // Связь с ProductType оставляем, но убираем связь с Remnant
                entity.HasOne<ProductType>()
                    .WithMany()
                    .HasForeignKey(e => e.IDType)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Stock
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.IDStock);
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.StockName)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsRequired(false);
                entity.Property(e => e.Schedule)
                    .HasMaxLength(200)
                    .IsRequired(false);
                entity.Property(e => e.FIASId)
                    .HasMaxLength(50)
                    .IsRequired(false);
                entity.Property(e => e.OwnerInn)
                    .HasMaxLength(20)
                    .IsRequired(false);
                entity.Property(e => e.OwnerKpp)
                    .HasMaxLength(20)
                    .IsRequired(false);
                entity.Property(e => e.OwnerFullName)
                    .HasMaxLength(500)
                    .IsRequired(false);
                entity.Property(e => e.OwnerShortName)
                    .HasMaxLength(200)
                    .IsRequired(false);
                entity.Property(e => e.RailwayStation)
                    .HasMaxLength(100)
                    .IsRequired(false);
                entity.Property(e => e.ConsigneeCode)
                    .HasMaxLength(50)
                    .IsRequired(false);
            });

            // Price (составной ключ)
            modelBuilder.Entity<Price>(entity =>
            {
                entity.HasKey(e => new { e.ID, e.IDStock });

                entity.Property(e => e.IDStock)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.PriceT)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.PriceT1)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceT2)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceM)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.PriceM1)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceM2)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceLimitT1)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceLimitT2)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceLimitM1)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.PriceLimitM2)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.NDS)
                    .HasColumnType("decimal(5,2)");

                // Связи оставляем, но без навигационных свойств в моделях
                entity.HasOne<Nomenclature>()
                    .WithMany()
                    .HasForeignKey(e => e.ID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Stock>()
                    .WithMany()
                    .HasForeignKey(e => e.IDStock)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Remnant (составной ключ) - УПРОЩЕННАЯ КОНФИГУРАЦИЯ БЕЗ НАВИГАЦИОННЫХ СВОЙСТВ
            modelBuilder.Entity<Remnant>(entity =>
            {
                entity.HasKey(e => new { e.ID, e.IDStock });

                entity.Property(e => e.IDStock)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.InStockT)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.InStockM)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.SoonArriveT)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.SoonArriveM)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.ReservedT)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.ReservedM)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);
                entity.Property(e => e.AvgTubeLength)
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.AvgTubeWeight)
                    .HasColumnType("decimal(10,2)");

                // УБРАНЫ все навигационные свойства для избежания циклических ссылок
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.TelegramUserId)
                    .IsRequired();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Inn)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);

                entity.Property(e => e.AdminNotified)
                    .IsRequired();

                // Owned types для OrderCartItem
                entity.OwnsMany(e => e.Items, owned =>
                {
                    owned.WithOwner();
                    owned.Property(i => i.ProductId)
                        .IsRequired();
                    owned.Property(i => i.ProductName)
                        .IsRequired()
                        .HasMaxLength(500);
                    owned.Property(i => i.Quantity)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    owned.Property(i => i.FinalPrice)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    owned.Property(i => i.UnitPrice)
                        .HasColumnType("decimal(18,2)")
                        .IsRequired();
                    owned.Property(i => i.IsInMeters)
                        .IsRequired();
                });
            });

            // CartItem (отдельная сущность для активной корзины)
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProductId, e.StockId, e.IsInMeters });

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .IsRequired();

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.IsInMeters)
                    .IsRequired();

                entity.Property(e => e.FinalPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.AddedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired();

                // Связь с Nomenclature
                entity.HasOne<Nomenclature>()
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_CartItem_UserId");

                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("IX_CartItem_ProductId");
            });

            // CustomerInfo
            modelBuilder.Entity<CustomerInfo>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Inn)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired();

                entity.HasIndex(e => e.UserId)
                    .IsUnique()
                    .HasDatabaseName("IX_CustomerInfo_UserId");
            });

            // TelegramUser
            modelBuilder.Entity<TelegramUser>(entity =>
            {
                entity.HasKey(e => e.TelegramUserId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Inn).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasIndex(e => e.Inn).HasDatabaseName("IX_TelegramUser_Inn");
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_TelegramUser_Email");
                entity.HasIndex(e => e.Phone).HasDatabaseName("IX_TelegramUser_Phone");
            });

            // PriceUpdate
            modelBuilder.Entity<PriceUpdate>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.StockId).IsRequired().HasMaxLength(36);
                entity.HasIndex(e => new { e.ProductId, e.StockId });
                entity.HasIndex(e => e.IsProcessed);
            });

            // RemnantUpdate
            modelBuilder.Entity<RemnantUpdate>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.StockId).IsRequired().HasMaxLength(36);
                entity.HasIndex(e => new { e.ProductId, e.StockId });
                entity.HasIndex(e => e.IsProcessed);
            });

            // StockUpdate
            modelBuilder.Entity<StockUpdate>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.StockId).IsRequired().HasMaxLength(36);
                entity.HasIndex(e => e.StockId);
                entity.HasIndex(e => e.IsProcessed);
            });

            // UpdateLog
            modelBuilder.Entity<UpdateLog>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.EntityType);
            });

            // Индексы для оптимизации
            modelBuilder.Entity<Nomenclature>()
                .HasIndex(e => e.IDType)
                .HasDatabaseName("IX_Nomenclature_IDType");

            modelBuilder.Entity<Price>()
                .HasIndex(e => e.IDStock)
                .HasDatabaseName("IX_Price_IDStock");

            modelBuilder.Entity<Remnant>()
                .HasIndex(e => e.IDStock)
                .HasDatabaseName("IX_Remnant_IDStock");

            modelBuilder.Entity<Order>()
                .HasIndex(e => e.TelegramUserId)
                .HasDatabaseName("IX_Order_TelegramUserId");

            modelBuilder.Entity<Order>()
                .HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Order_CreatedAt");
        }
    }
}