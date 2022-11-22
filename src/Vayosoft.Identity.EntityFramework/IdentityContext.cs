using Microsoft.EntityFrameworkCore;
using Vayosoft.Identity.Security;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Identity.EntityFramework
{
    public sealed class IdentityContext : DbContext
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

            var userEntity = modelBuilder.Entity<UserEntity>();
            {
                userEntity
                    .Property(t => t.Username); //read_only field
                userEntity
                    .HasIndex(u => u.Username).IsUnique();
                userEntity.HasData(
                    new UserEntity("su")
                    {
                        Id = 1,
                        PasswordHash = "VBbXzW7xlaD3YiqcVrVehA==",
                        Phone = "0500000000",
                        Email = "su@vayosoft.com",
                        Type = UserType.Supervisor,
                        Registered = new DateTime(2022, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                        CultureId = "ru-RU",
                        ProviderId = 1000,
                    }
                );
            }

            var refreshToken = modelBuilder.Entity<RefreshToken>();
            {
                refreshToken
                    .HasKey(t => new {t.UserId, t.Token});
                refreshToken
                    .HasOne(t => t.User as UserEntity)
                    .WithMany(t => t.RefreshTokens)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            }

            var securityObject = modelBuilder.Entity<SecurityObjectEntity>();
            {
                securityObject.ToTable("sec_objs").HasKey(t => t.Id);
                securityObject.Property(t => t.Id).HasColumnName("objid").ValueGeneratedOnAdd();
                securityObject.Property(t => t.Name).HasColumnName("obj_name").IsRequired();
                securityObject.Property(t => t.Description).HasColumnName("obj_desc");
            }

            var securityRole = modelBuilder.Entity<SecurityRoleEntity>();
            {
                securityRole.ToTable("sec_roles").HasKey(t => t.Id);
                securityRole.Property(t => t.Id).HasColumnName("roleid").ValueGeneratedOnAdd();
                securityRole.Property(t => t.ProviderId).HasColumnName("providerid");
                securityRole.Property(t => t.Name).HasColumnName("role_name").IsRequired();
                securityRole.Property(t => t.Description).HasColumnName("role_desc");
            }

            var securityRolePermissions = modelBuilder.Entity<SecurityRolePermissionsEntity>();
            {
                securityRolePermissions.ToTable("sec_role_permissions").HasKey(t => t.Id);
                securityRolePermissions.Property(t => t.Id).HasColumnName("permid").ValueGeneratedOnAdd();
                securityRolePermissions.Property(t => t.RoleId).HasColumnName("roleid").IsRequired();
                securityRolePermissions.Property(t => t.ObjectId).HasColumnName("objid").IsRequired();
                securityRolePermissions.Property(t => t.Permissions).HasColumnName("perms").IsRequired();
            }

            var userRole = modelBuilder.Entity<UserRoleEntity>();
            {
                userRole.ToTable("sec_user_roles").HasKey(t => t.Id);
                userRole.Property(t => t.Id).HasColumnName("urid").ValueGeneratedOnAdd();
                userRole.Property(t => t.RoleId).HasColumnName("userid").IsRequired();
                userRole.Property(t => t.RoleId).HasColumnName("roleid").IsRequired();
            }
        }
    }
}
