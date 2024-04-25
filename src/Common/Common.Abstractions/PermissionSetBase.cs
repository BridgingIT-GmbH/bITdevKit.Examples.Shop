namespace Common;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public abstract class PermissionSetBase : IPermissionSet
{
    public virtual IEnumerable<string> GetPermissions()
    {
        var results = new List<string>();
        foreach (var prop in this.GetType().GetNestedTypes()
            .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
            {
                results.Add(propertyValue.ToString());
            }
        }

        return results;
    }
}
