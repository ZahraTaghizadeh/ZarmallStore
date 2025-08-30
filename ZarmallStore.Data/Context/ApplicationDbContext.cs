using Microsoft.EntityFrameworkCore;
using ZarmallStore.Data.Entities.Account;

namespace ZarmallStore.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        #region Account
        public DbSet<User> Users { get; set; }
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
        }
        #endregion
    }
}
