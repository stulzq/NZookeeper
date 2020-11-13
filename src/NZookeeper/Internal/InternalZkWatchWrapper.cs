using System.Threading.Tasks;
using NZookeeper.Enums;
using org.apache.zookeeper;

namespace NZookeeper.Internal
{
    internal class InternalZkWatchWrapper : Watcher
    {
        private readonly ZkConnection.WatchHandle _handler;

        public InternalZkWatchWrapper(ZkConnection.WatchHandle handler)
        {
            _handler = handler;
        }
        public override Task process(WatchedEvent ev)
        {
            return _handler?.Invoke(new ZkWatchEventArgs(){Path = ev.getPath(),EventType = (WatchEventType)(int)ev.get_Type(),State = (ZkState)(int)ev.getState() });
        }
    }
}