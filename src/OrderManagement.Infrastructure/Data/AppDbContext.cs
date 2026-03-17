using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<LogAuditory> LogAuditory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderHeader>(entity =>
            {
                entity.ToTable("OrderHeaders");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ClientId).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Total).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.UserName).HasMaxLength(255).IsRequired();

                entity.HasMany(e => e.Details)
                    .WithOne(e => e.OrderHeader)
                    .HasForeignKey(e => e.OrderHeaderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();

                entity.HasOne(e => e.OrderHeader)
                    .WithMany(e => e.Details)
                    .HasForeignKey(e => e.OrderHeaderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                    .WithMany(e => e.OrderDetails)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LogAuditory>(entity =>
            {
                entity.ToTable("LogAuditory");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Event).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            });
        }
    }
}