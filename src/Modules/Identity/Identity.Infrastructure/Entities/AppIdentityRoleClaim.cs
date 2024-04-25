namespace Modules.Identity.Infrastructure;

using Microsoft.AspNetCore.Identity;

public class AppIdentityRoleClaim : IdentityRoleClaim<Guid>
{
    public AppIdentityRoleClaim()
    {
    }

    public AppIdentityRoleClaim(string roleClaimDescription = null, string roleClaimGroup = null)
    {
        this.Description = roleClaimDescription;
        this.Group = roleClaimGroup;
    }

    public string Description { get; set; }

    public string Group { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual AppIdentityRole Role { get; set; }
}
