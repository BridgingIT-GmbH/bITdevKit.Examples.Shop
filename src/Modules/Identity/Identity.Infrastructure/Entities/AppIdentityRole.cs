namespace Modules.Identity.Infrastructure;

using Microsoft.AspNetCore.Identity;

public class AppIdentityRole : IdentityRole<Guid>
{
    public AppIdentityRole()
    {
        this.RoleClaims = new HashSet<AppIdentityRoleClaim>();
    }

    public AppIdentityRole(string roleName, string roleDescription = null)
        : base(roleName)
    {
        this.RoleClaims = new HashSet<AppIdentityRoleClaim>();
        this.Description = roleDescription;
    }

    public string Description { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<AppIdentityRoleClaim> RoleClaims { get; set; }
}
