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

        public Monitoring(IHubContext<Hubs.MonitoringHub, Interfaces.IMonitoringClient> hubContext, IConfiguration config)
        {
            _hubContext = hubContext;
            _config = config;

            _monitor = async () =>
            {
                while (Clients.Count > 0)
                {
                    await _hubContext.Clients.All.UpdateTime(CurrentTime);
                    await Task.Delay(100);
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
    }
}
