using System.Threading.Tasks;

namespace Interfaces
{
    public interface IMonitoringClient
    {
        Task UpdateTime(string currentTime);
        Task UpdateAvailableDiskSpace(double space);
        Task RunBitcoinNode();
        Task DownBitcoinNode();
        public Task UpdateBitcoinNodeState(string state);
        Task UpdateBitcoinBlockCount(int count);
    }
}
