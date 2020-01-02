using System.Threading.Tasks;

namespace Interfaces
{
    public interface IMonitoringClient
    {
        public Task UpdateTime(string currentTime);
    }
}