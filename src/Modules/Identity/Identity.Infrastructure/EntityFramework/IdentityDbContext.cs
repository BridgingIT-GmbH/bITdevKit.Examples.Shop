namespace Modules.Identity.Infrastructure.EntityFramework;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

public class IdentityDbContext :
    IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, AppIdentityRoleClaim, IdentityUserToken<Guid>>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug)));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(
            this.GetType().Name.ToLowerInvariant().Replace("dbcontext", string.Empty, StringComparison.OrdinalIgnoreCase));

        base.OnModelCreating(modelBuilder);
    }
}
