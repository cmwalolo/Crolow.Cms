using Crolow.Cms.Server.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using static Crolow.Cms.Server.Common.SignalR.HubCache;

/// <summary>
///  to install signalR on IIS : install new feature on iis 
///  install websocket protocol
/// </summary>
namespace Crolow.Cms.Server.Common.SignalR
{
    public class BaseHub : Hub
    {
        protected static readonly string GroupTypeDefault = "All";
        protected static HubCache hubCache = new HubCache();

        public BaseHub()
        {
        }

        #region protected Methods
        protected Client CheckClient(string connectionId)
        {
            var client = hubCache.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (client == null)
            {
                throw new SecurityTokenEncryptionKeyNotFoundException();
            }

            return client;
        }

        protected Group CheckGroup(string groupName)
        {
            return hubCache.Groups.FirstOrDefault(c => c.GroupName == groupName);
        }

        protected Client GetClient(string userName)
        {
            return hubCache.Clients.FirstOrDefault(c => c.Username == userName);
        }


        protected async Task AddGroupWithoutMessage(Client client, string groupName, string groupType)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = new Group { GroupName = groupName, GroupType = groupType };
            hubCache.Groups.Add(group);
            if (client != null)
            {
                group.Clients.Add(client);
            }
        }

        protected async Task AddToGroupWithoutMessage(Client client, string groupName)
        {
            if (client != null)
            {
                await Groups.AddToGroupAsync(client.ConnectionId, groupName);
                var group = hubCache.Groups.FirstOrDefault(p => p.GroupName.Equals(groupName));
                if (group != null)
                {
                    group.Clients.Add(client);
                }
            }
        }
        #endregion


        /// <summary>
        /// Add a user and associated object to the connection pool
        /// </summary>
        /// <param name="username"></param>
        /// <param name="clientModel"></param>
        public void AddUser(string username, object clientModel)
        {
            Client client = new Client { ConnectionId = Context.ConnectionId, Username = username, ClientReference = clientModel };
            hubCache.Clients.Add(client);
        }


        /// <summary>
        /// If the client is for some reason disconnected
        /// We can reset the connection ID for this client
        /// </summary>
        /// <param name="username"></param>
        /// <param name="clientModel"></param>
        public void ResetUser(string userName)
        {
            var client = CheckClient(userName);
            if (client != null)
            {
                client.ConnectionId = Context.ConnectionId;
            }
        }

        public async Task RemoveUser(string groupType)
        {
            var client = CheckClient(Context.ConnectionId);
            if (client != null)
            {
                hubCache.Clients.Remove(client);
                foreach (var group in hubCache.Groups.Where(p => string.IsNullOrEmpty(groupType) || p.GroupType == groupType).ToList())
                {
                    var clientCached = group.Clients.First(p => p.Username.Equals(client.Username));
                    if (clientCached != null)
                    {
                        group.Clients.Remove(client);
                        await RemoveFromGroup(group.GroupName);
                    }
                }
            }
        }

        public async Task ListUsers()
        {
            var client = CheckClient(Context.ConnectionId);
            await Clients.All.SendAsync(BaseHubMessages.ListUsers.ToString(), hubCache.Clients.Select(p => p.Username).ToArray());
        }

        public async Task AddGroup(string groupName, string groupType = "All")
        {
            var client = CheckClient(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = new Group { GroupName = groupName, GroupType = groupType };
            group.Clients.Add(client);
            hubCache.Groups.Add(group);
            await Clients.All.SendAsync(BaseHubMessages.NewGroup.ToString(), groupName);
            await Clients.All.SendAsync(BaseHubMessages.JoinGroup.ToString(), groupName, client.Username);
        }

        public async Task AddToGroup(string groupName)
        {
            var client = CheckClient(Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = hubCache.Groups.FirstOrDefault(p => p.GroupName.Equals(groupName));
            if (group != null && !group.Clients.Contains(client))
            {
                group.Clients.Add(client);
                await Clients.All.SendAsync(BaseHubMessages.JoinGroup.ToString(), groupName, client.Username);
            }
        }

        public async Task RemoveFromGroup(string groupName)
        {
            var client = CheckClient(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = hubCache.Groups.FirstOrDefault(p => p.GroupName.Equals(groupName));
            if (group != null)
            {
                group.Clients.Remove(client);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.Groups(group.GroupName).SendAsync(BaseHubMessages.QuitGroup.ToString(), groupName, client.Username);
            }
        }

        public async Task RemoveGroup(string groupName)
        {
            var client = CheckClient(Context.ConnectionId);
            var group = hubCache.Groups.FirstOrDefault(p => p.GroupName.Equals(groupName));
            if (group != null)
            {
                hubCache.Groups.Remove(group);
                await Clients.All.SendAsync(BaseHubMessages.RemoveGroup.ToString(), groupName);
            }
        }

        public async Task ListGroup(string groupName, string groupType = "All")
        {
            var client = CheckClient(Context.ConnectionId);
            Group group = hubCache.Groups.FirstOrDefault(g => g.GroupType == groupType && g.GroupName.Equals(groupName));
            if (group != null)
            {
                await Clients.Groups(groupName).SendAsync(BaseHubMessages.ListGroup.ToString(), group.Clients.Select(p => p.Username).ToArray());
            }
        }

        #region Basic Messages 
        public async Task SendMessageToUser(string method, object message)
        {
            var client = CheckClient(Context.ConnectionId);
            await Clients.Client(client.ConnectionId).SendAsync(method, message);
        }

        public async Task SendMessageToAll(string method, object message)
        {
            var client = CheckClient(Context.ConnectionId);
            await Clients.All.SendAsync(method, message);
        }

        public async Task SendMessageToGroup(string group, string method, object message)
        {
            var client = CheckClient(Context.ConnectionId);
            await Clients.Group(group).SendAsync(method, message);
        }
        #endregion
    }
}
