namespace BridgingIT.DevKit.Application.Collaboration;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}