namespace Interfaces
{
    public interface IMonitoring
    {
        double AvailableFreeDiskSpace{ get; }
        string CurrentTime { get; }
        void TryStartup();
    }
}