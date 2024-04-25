namespace Modules.Identity.Application;

public interface ICurrentUserService
{
    string UserId { get; }

    List<KeyValuePair<string, string>> Claims { get; set; }
}