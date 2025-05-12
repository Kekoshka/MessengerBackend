using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessengerWebAPIBackend.Hubs
{
    [Authorize]
    public class MessengerHub : Hub
    {
    }
}