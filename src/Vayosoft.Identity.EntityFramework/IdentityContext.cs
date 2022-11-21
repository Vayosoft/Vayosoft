using Microsoft.EntityFrameworkCore;
using Vayosoft.Persistence.EntityFramework;

namespace Vayosoft.Identity.EntityFramework
{
    public sealed class IdentityContext : DataContext
    {
        public IdentityContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity("su")
                {
                    Id = 0,
                    PasswordHash = "VBbXzW7xlaD3YiqcVrVehA==",
                    Phone= "0500000000",
                    Type = UserType.Supervisor,
                    Registered = DateTime.UtcNow,
                    ProviderId = 1000,
                }
            );
        }
    }
}
