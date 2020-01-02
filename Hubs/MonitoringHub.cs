using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace Hubs
{
    public class MonitoringHub : Hub<Interfaces.IMonitoringClient>
    {
        private Interfaces.IMonitoring _model;

        public MonitoringHub(Interfaces.IMonitoring model)
        {
            _model = model;
        }

        public override async Task OnConnectedAsync()
        {
            Models.Monitoring.Clients.TryAdd(Context.ConnectionId, 0);
            _model.TryStartup();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Models.Monitoring.Clients.TryRemove(Context.ConnectionId, out byte value);
            await base.OnDisconnectedAsync(exception);

        }

        public async Task UpdateTime(string currentTime)
        {
            await Clients.All.UpdateTime(currentTime);
        }
    }
}
