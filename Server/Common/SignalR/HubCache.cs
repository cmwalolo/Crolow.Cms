using System.Collections.Generic;

//using TopMachine.Topping.Core.Models.Game.Responses;
//using TopMachine.Topping.Core.Models.Rooms.Responses;

/// <summary>
///  to install signalR on IIS : install new feature on iis 
///  install websocket protocol
/// </summary>
namespace Crolow.Cms.Server.Common.SignalR
{
    public class HubCache
    {
        public class Client
        {
            public string ConnectionId { get; set; }
            public string Username { get; set; }
            public object ClientReference { get; set; }
        }

        public class Group
        {
            public string GroupType { get; set; }
            public string GroupName { get; set; }
            public List<Client> Clients { get; } = new List<Client>();
        }

        public List<Client> Clients { get; } = new List<Client>();
        public List<Group> Groups { get; } = new List<Group>();
    }
}
