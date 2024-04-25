namespace Modules.Ordering.Application;

using BridgingIT.DevKit.Application.Messaging;

public class EchoMessage : MessageBase
{
    public EchoMessage()
    {
    }

    public EchoMessage(string text)
    {
        this.Text = text;
    }

    public string Text { get; set; }
}