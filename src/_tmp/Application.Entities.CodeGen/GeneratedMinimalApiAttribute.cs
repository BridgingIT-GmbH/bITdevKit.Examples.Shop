namespace BridgingIT.DevKit.Application.Entities;

using System;

/// <summary>
/// Base entity method attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal class GeneratedMinimalApiAttribute : Attribute // TODO: move to Application.Entities?
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedMinimalApiAttribute"/> class.
    /// </summary>
    public GeneratedMinimalApiAttribute()
        : this(string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedMinimalApiAttribute"/> class.
    /// </summary>
    /// <param name="template">The route template.</param>
    public GeneratedMinimalApiAttribute(string template)
    {
        this.Template = template;
    }

    /// <summary>
    /// Gets or sets the module.
    /// </summary>
    public string Module { get; set; }

    /// <summary>
    /// Gets or sets the route template.
    /// </summary>
    public string Template { get; set; } = "api";

    /// <summary>
    /// Gets or sets if authorization is needed.
    /// </summary>
    public bool Authorize { get; set; }

    /// <summary>
    /// Gets or sets the authorization policy.
    /// </summary>
    public string Policy { get; set; }

    /// <summary>
    /// Gets or sets the extra parameters.
    /// </summary>
    public string[] Parameters { get; set; }
}
