//using TopMachine.Topping.Core.Models.Game.Responses;
//using TopMachine.Topping.Core.Models.Rooms.Responses;

/// <summary>
///  to install signalR on IIS : install new feature on iis 
///  install websocket protocol
/// </summary>
namespace Crolow.Cms.Server.SignalR
{
    public enum BaseHubMessages
    {
        ListUsers = 0,

        NewGroup = 10,
        JoinGroup = 11,
        QuitGroup = 12,
        ListGroup = 13,
        RemoveGroup = 14
    }
}
