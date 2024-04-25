namespace Modules.Ordering.Domain;

using System.Text.RegularExpressions;
using System.Threading;
using BridgingIT.DevKit.Domain;

public class IsValidEmailAddress : IBusinessRule
{
    private static readonly Regex Regex = new(
        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
        RegexOptions.Compiled);

    private readonly string value;

    public IsValidEmailAddress(string value)
    {
        this.value = value?.ToLowerInvariant();
    }

    public string Message => "Not a valid email address";

    public Task<bool> IsSatisfiedAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(!string.IsNullOrEmpty(this.value) && this.value.Length <= 255 && Regex.IsMatch(this.value));
}
