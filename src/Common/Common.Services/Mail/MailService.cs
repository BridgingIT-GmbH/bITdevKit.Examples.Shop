namespace Common.Services;

using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

public class MailService : IMailService
{
    private readonly MailServiceOptions options;
    private readonly ILogger<MailService> logger;

    public MailService(MailServiceOptions options)
    {
        EnsureArg.IsNotNull(options, nameof(options));
        EnsureArg.IsNotNullOrEmpty(options.Host, nameof(options.Host));

        this.options = options;
        this.logger = options.CreateLogger<MailService>();
    }

    public MailService(Builder<MailServiceOptionsBuilder, MailServiceOptions> optionsBuilder)
        : this(optionsBuilder(new MailServiceOptionsBuilder()).Build())
    {
    }

    public async Task SendAsync(MailRequest request)
    {
        if (!this.options.Enabled)
        {
            this.logger.LogInformation("mail: skip send (enabled=false)");
            return;
        }

        // TODO: validate MailRequest (FluentValidation)

        try
        {
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(this.options.DisplayName, request.Sender ?? this.options.Sender),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.HtmlBody,
                    TextBody = request.TextBody
                }.ToMessageBody()
            };

            if (!string.IsNullOrEmpty(request.Recipient))
            {
                email.To.Add(MailboxAddress.Parse(request.Recipient));
            }

            foreach (var recipient in request.Recipients.SafeNull())
            {
                email.To.Add(MailboxAddress.Parse(recipient));
            }

            foreach (var recipient in request.Cc.SafeNull())
            {
                email.Cc.Add(MailboxAddress.Parse(recipient));
            }

            foreach (var recipient in request.Bcc.SafeNull())
            {
                email.Bcc.Add(MailboxAddress.Parse(recipient));
            }

            this.logger.LogInformation("mail: send (host={mailHost})", $"{this.options.Host}:{this.options.Port}");

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(this.options.Host, this.options.Port, SecureSocketOptions.Auto);
            if (!string.IsNullOrEmpty(this.options.UserName))
            {
                await smtp.AuthenticateAsync(this.options.UserName, this.options.Password);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ""); // TODO: ignore??
        }
    }
}