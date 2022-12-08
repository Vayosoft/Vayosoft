using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Enums;
using Vayosoft.Identity.Security;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Identity.EntityFramework
{
    public sealed class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options) { }

        //public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<UserEntity> Users { set; get; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userEntity = modelBuilder.Entity<UserEntity>();
            {
                userEntity.ToTable("users").HasKey(t => t.Id);
                userEntity.Property(t => t.Id).HasColumnName("userid");
                userEntity.Property(t => t.Username).HasColumnName("username").IsRequired();
                userEntity.Property(t => t.Email).HasColumnName("email");
                userEntity.Property(t => t.PasswordHash).HasColumnName("pwdhash");
                userEntity.Property(t => t.Phone).HasColumnName("phone").IsRequired();
                userEntity.Property(t => t.Type).HasColumnName("user_type");
                userEntity.Property(t => t.ProviderId).HasColumnName("providerid");
                userEntity.Property(t => t.LogLevel).HasColumnName("log_level").HasDefaultValue(LogEventType.Error);
                userEntity.Property(t => t.CultureId).HasColumnName("culture_id").HasDefaultValue("he-IL");
                userEntity.Property(t => t.Registered).HasColumnName("regdate").IsRequired();
                userEntity.Property(t => t.Deregistered).HasColumnName("enddate");

                userEntity
                    .HasIndex(u => new { u.Username, u.ProviderId }).IsUnique();
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
                        ProviderId = 0,
                    }
                );
            }

            var refreshToken = modelBuilder.Entity<RefreshToken>();
            {
                refreshToken.ToTable("refresh_tokens").HasKey(t => new { t.UserId, t.Token });
                refreshToken.Property(t => t.UserId).HasColumnName("userid").IsRequired();
                refreshToken.Property(t => t.Token).HasColumnName("token").IsRequired();
                refreshToken.Property(t => t.Created).HasColumnName("created");
                refreshToken.Property(t => t.CreatedByIp).HasColumnName("created_by_ip");
                refreshToken.Property(t => t.Revoked).HasColumnName("revoked");
                refreshToken.Property(t => t.RevokedByIp).HasColumnName("revoked_by_ip");
                refreshToken.Property(t => t.ReasonRevoked).HasColumnName("reason_revoked");
                refreshToken.Property(t => t.ReplacedByToken).HasColumnName("replaced_by_token");
                refreshToken.Property(t => t.Expires).HasColumnName("expires");

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
