namespace Common.SignalR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class SignalRHub : Hub // TODO: rename to AppHub?
{
    public async Task PingRequestAsync(string userId)
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.PingRequest, userId);
    }

    public async Task PingResponseAsync(string userId, string requestedUserId)
    {
        await this.Clients.User(requestedUserId).SendAsync(SignalRHubConstants.PingResponse, userId);
    }

    public async Task OnConnectAsync(string userId)
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.ConnectUser, userId);
    }

    public async Task OnDisconnectAsync(string userId)
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.DisconnectUser, userId);
    }

    public async Task OnDeactivateUser(string userId)
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.LogoutDeactivatedUser, userId);
    }

    public async Task OnChangeRolePermissions(string userId, string roleId) // used in: RolePermissions::SaveAsync() >> await this.HubConnection.SendAsync(HubConstants.OnChangeRolePermissions,  ....
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.LogoutUsersByRole, userId, roleId);
    }

    //public async Task SendMessageAsync(ChatHistory<IChatUser> chatHistory, string userName)
    //{
    //    await Clients.User(chatHistory.ToUserId).SendAsync(HubConstants.ReceiveMessage, chatHistory, userName);
    //    await Clients.User(chatHistory.FromUserId).SendAsync(HubConstants.ReceiveMessage, chatHistory, userName);
    //}

    //public async Task ChatNotificationAsync(string message, string receiverUserId, string senderUserId)
    //{
    //    await Clients.User(receiverUserId).SendAsync(HubConstants.ReceiveChatNotification, message, receiverUserId, senderUserId);
    //}

    //public async Task SendInformationMessageAsync(string message)
    //{
    //    await this.Clients.All.SendAsync(HubConstants.ReceiveInformationMessage, message);
    //}

    public async Task UpdateDashboardAsync()
    {
        await this.Clients.All.SendAsync(SignalRHubConstants.ReceiveUpdateDashboard);
    }

    //public async Task RegenerateTokensAsync()
    //{
    //    await Clients.All.SendAsync(HubConstants.ReceiveRegenerateTokens);
    //}
}