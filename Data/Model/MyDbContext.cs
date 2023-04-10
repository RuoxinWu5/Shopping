using Microsoft.EntityFrameworkCore;

namespace Data.Model
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BuyerProduct> BuyerProducts { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.User);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.User)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SellerId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => p.Quantity > 0);

            modelBuilder.Entity<BuyerProduct>().HasNoKey().ToView("BuyerProduct")
                .Property(bp => bp.Id).HasColumnName("Id");
            modelBuilder.Entity<BuyerProduct>().Property(bp => bp.Name).HasColumnName("Name");
            modelBuilder.Entity<BuyerProduct>().Property(bp => bp.Quantity).HasColumnName("Quantity");
            modelBuilder.Entity<BuyerProduct>().Property(bp => bp.SellerName).HasColumnName("SellerName");
            modelBuilder.Entity<BuyerProduct>().HasQueryFilter(bp => bp.Quantity > 0);
            modelBuilder.Entity<BuyerProduct>()
                .HasQueryFilter(bp => bp.Quantity > 0);
        }
    }
}