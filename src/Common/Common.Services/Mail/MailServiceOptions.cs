namespace Common.Services;

using BridgingIT.DevKit.Common;

public class MailServiceOptions : OptionsBase
{
    public bool Enabled { get; set; } = true;

    public string Sender { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string DisplayName { get; set; }
}