namespace NZookeeper.Enums
{
    public enum WatchEventType
    {
        /// <summary/>
        None = -1,
        /// <summary>
        /// a node created
        /// </summary>
        NodeCreated = 1,
        /// <summary>
        /// a node deleted
        /// </summary>
        NodeDeleted = 2,
        /// <summary>
        /// a node data changed
        /// </summary>
        NodeDataChanged = 3,
        /// <summary>
        /// a node children changed
        /// </summary>
        NodeChildrenChanged = 4

    }
}