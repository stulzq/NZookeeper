using NZookeeper.Node;

namespace NZookeeper.Data
{
    public class DataResult<T>
    {
        public DataResult(T data, NodeStatus nodeStatus)
        {
            Data = data;
            NodeStatus = nodeStatus;
        }
        public T Data { get; }
        public NodeStatus NodeStatus { get; }
    }
}