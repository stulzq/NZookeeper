using org.apache.zookeeper.data;

namespace NZookeeper.Node
{
    public class NodeStatus
    {
        private readonly Stat _stat;

        public NodeStatus(Stat stat)
        {
            _stat = stat;
        }

        public long Czxid => _stat.getCzxid();
        public long Mzxid => _stat.getMzxid();
        public long Ctime => _stat.getCtime();
        public long Mtime => _stat.getMtime();
        public int Version => _stat.getVersion();
        public int Cversion => _stat.getCversion();
        public int Aversion => _stat.getAversion();
        public long EphemeralOwner => _stat.getEphemeralOwner();
        public int DataLength => _stat.getDataLength();
        public int NumChildren => _stat.getNumChildren();
        public long Pzxid => _stat.getPzxid();
    }
}