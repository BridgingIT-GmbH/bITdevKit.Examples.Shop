namespace Common.Services;

using BridgingIT.DevKit.Common;
using Microsoft.Extensions.Logging;

public class MailServiceOptionsBuilder :
    OptionsBuilderBase<MailServiceOptions, MailServiceOptionsBuilder>
{
    public MailServiceOptionsBuilder Enable(bool enabled)
    {
        this.Target.Enabled = enabled;
        return this;
    }

    public MailServiceOptionsBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
    {
        this.Target.LoggerFactory = loggerFactory;
        return this;
    }

    public MailServiceOptionsBuilder Host(string host, int port = 25)
    {
        this.Target.Host = host;
        this.Target.Port = port;
        return this;
    }

    public MailServiceOptionsBuilder Authentication(string userName, string password)
    {
        this.Target.UserName = userName;
        this.Target.Password = password;
        return this;
    }

    public MailServiceOptionsBuilder Sender(string name, string address)
    {
        this.Target.Sender = address;
        this.Target.DisplayName = name;
        return this;
    }
}