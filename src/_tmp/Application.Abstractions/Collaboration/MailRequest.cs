namespace BridgingIT.DevKit.Application.Collaboration;

public class MailRequest
{
    public string Sender { get; set; }

    public string Recipient { get; set; }

    public IEnumerable<string> Recipients { get; set; }

    public IEnumerable<string> Cc { get; set; }

    public IEnumerable<string> Bcc { get; set; }

    public string Subject { get; set; }

    public string TextBody { get; set; }

    public string HtmlBody { get; set; }
}