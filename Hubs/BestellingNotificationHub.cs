using Microsoft.AspNetCore.SignalR;

public class BestellingNotificationHub : Hub
{
    public async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
}