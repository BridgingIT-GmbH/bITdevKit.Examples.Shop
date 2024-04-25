namespace Common;

using System.Collections.Generic;

public interface IPermissionSet
{
    IEnumerable<string> GetPermissions();
}