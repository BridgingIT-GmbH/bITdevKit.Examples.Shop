﻿namespace Modules.Identity.Application;

public class RoleClaimModel
{
    public int Id { get; set; }

    public string RoleId { get; set; }

    public string Type { get; set; }

    public string Value { get; set; }

    public string Description { get; set; }

    public string Module { get; set; }

    public string Group { get; set; }

    public bool Selected { get; set; }
}
