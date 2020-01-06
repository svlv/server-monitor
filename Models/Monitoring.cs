using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Models
{
    public class Monitoring : Interfaces.IMonitoring
    {
        public static ConcurrentDictionary<string, byte> Clients = new ConcurrentDictionary<string, byte>();
        private readonly IHubContext<Hubs.MonitoringHub, Interfaces.IMonitoringClient> _hubContext;
        private readonly IConfiguration _config;
        private Task _monitoringTask;
        private readonly Func<Task> _monitor;
        private readonly Bitcoin.Node _node;
        private readonly Bitcoin.RpcClient _rpc;

        public Monitoring(IHubContext<Hubs.MonitoringHub, Interfaces.IMonitoringClient> hubContext, IConfiguration config)
        {
            _hubContext = hubContext;
            _config = config;

            _node = new Bitcoin.Node();
            _rpc = new Bitcoin.RpcClient()
            {
                UserName = _config.GetSection("Bitcoin").GetSection("Rpc").GetSection("UserName").Value,
                Password = _config.GetSection("Bitcoin").GetSection("Rpc").GetSection("Password").Value,
                Uri      = _config.GetSection("Bitcoin").GetSection("Rpc").GetSection("Uri").Value
            };

            _monitor = async () =>
            {
                while (Clients.Count > 0)
                {
                    await _hubContext.Clients.All.UpdateAvailableDiskSpace(AvailableFreeDiskSpace);
                    await _hubContext.Clients.All.UpdateTime(CurrentTime);
                    await _hubContext.Clients.All.UpdateBitcoinBlockCount(_rpc.GetBlockCount());
                    await Task.Delay(750);
                }
            };

            _monitoringTask = Task.Run(_monitor);
        }

        public void TryStartup()
        {
            switch (_monitoringTask.Status)
            {
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.Running:
                    break;
                default:
                    _monitoringTask = Task.Run(_monitor);
                    break;
            }
        }

        public string CurrentTime
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        public double AvailableFreeDiskSpace
        {
            get
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.Name == "/")
                    {
                        return Convert.ToDouble(drive.TotalFreeSpace) / Convert.ToDouble(1 << 30);
                    }
                }
                return 0;
            }
        }

        public void RunBitcoinNode()
        {
            _node.Run();
            _hubContext.Clients.All.UpdateBitcoinNodeState("Running");
        }

        public void DownBitcoinNode()
        {
            _node.Down();
            _hubContext.Clients.All.UpdateBitcoinNodeState("Stopped");
        }
    }
}
