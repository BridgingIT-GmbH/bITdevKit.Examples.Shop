namespace Modules.Identity.Application;

using System.ComponentModel.DataAnnotations;

public class RoleRequestModel
{
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }
}
