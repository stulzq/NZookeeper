using NZookeeper.Enums;

namespace NZookeeper
{
    public class ZkWatchEventArgs
    {
        public string Path { get; set; }
        public ZkState State { get; set; }
        public WatchEventType EventType { get; set; }
    }
}