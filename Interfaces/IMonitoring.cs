namespace Interfaces
{
    public interface IMonitoring
    {
        double AvailableFreeDiskSpace{ get; }
        string CurrentTime { get; }
        void TryStartup();
        void RunBitcoinNode();
        void DownBitcoinNode();
    }
}
