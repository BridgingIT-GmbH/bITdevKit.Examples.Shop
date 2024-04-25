namespace Common.SignalR;

public static class SignalRHubConstants // TODO: move to Common.Abstractions?
{
    public const string HubUrl = "/signalrhub";

    public const string SendUpdateDashboard = "UpdateDashboardAsync";
    public const string ReceiveUpdateDashboard = "UpdateDashboard";

    //public const string SendRegenerateTokens = "RegenerateTokensAsync";
    //public const string ReceiveRegenerateTokens = "RegenerateTokens";

    //public const string ReceiveChatNotification = "ReceiveChatNotification";
    //public const string SendChatNotification = "ChatNotificationAsync";
    //public const string ReceiveMessage = "ReceiveMessage";
    //public const string SendMessage = "SendMessageAsync";

    public const string ReceiveInformationMessage = "ReceiveInformationMessage";
    public const string SendInformationMessage = "SendInformationMessageAsync";

    public const string OnConnect = "OnConnectAsync";
    public const string ConnectUser = "ConnectUser";
    public const string OnDisconnect = "OnDisconnectAsync";
    public const string DisconnectUser = "DisconnectUser";
    public const string OnChangeRolePermissions = "OnChangeRolePermissions";
    public const string OnDeactivateUser = "OnDeactivateUser";
    public const string LogoutUsersByRole = "LogoutUsersByRole";
    public const string LogoutDeactivatedUser = "LogoutDeactivatedUser";

    public const string PingRequest = "PingRequestAsync";
    public const string PingResponse = "PingResponseAsync";
}