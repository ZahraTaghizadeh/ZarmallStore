using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.Common;
using ZarmallStore.Data.Entities.OrderEntities;
using ZarmallStore.Data.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;

namespace ZarmallStore.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        #region Account
        public DbSet<User> Users { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        #endregion

        #region Common
        public DbSet<SiteInfo> SiteInfos { get; set; }
        public DbSet<Banner> Banners { get; set; }
        #endregion

        #region Products
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSelectedCategory> ProductSelectedCategories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ProductSelectedBrand> ProductSelectedBrands { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }
        public DbSet<ProductGallery> ProductGalleries { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        #endregion

        #region Order 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<ProductSell> ProductSells { get; set; }
        #endregion

        #region FilterData
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductCategory>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductVariant>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductColor>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductComment>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductSelectedCategory>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Brand>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductSelectedBrand>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductFeature>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductGallery>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Order>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<OrderDetail>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<PaymentRecord>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<FavoriteProduct>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Banner>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<ProductSell>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<UserRole>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Role>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<Permission>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<RolePermission>()
                .HasQueryFilter(u => !u.IsDeleted);

            modelBuilder.Entity<SiteInfo>()
                .HasQueryFilter(u => !u.IsDeleted);
        }
        #endregion
    }
}
