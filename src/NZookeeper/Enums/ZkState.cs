namespace NZookeeper.Enums
{
    public enum ZkState
    {
        /// <summary>
        /// The client is in the disconnected state - it is not connected
        /// to any server in the ensemble.
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// The client is in the connected state - it is connected
        /// to a server in the ensemble (one of the servers specified
        /// in the host connection parameter during ZooKeeper client
        /// creation).
        /// </summary>
        SyncConnected = 3,
        /// <summary>
        /// The authentication failed
        /// </summary>
        AuthFailed = 4,
        /// <summary>
        /// The client is connected to a read-only server, that is the
        /// server which is not currently connected to the majority.
        /// The only operations allowed after receiving this state is
        /// read operations.
        /// This state is generated for read-only clients only since
        /// read/write clients aren't allowed to connect to r/o servers.
        /// </summary>
        ConnectedReadOnly = 5,
        /// <summary>
        /// The serving cluster has expired this session. The ZooKeeper
        /// client connection (the session) is no longer valid. You must
        /// create a new client connection (instantiate a new ZooKeeper
        /// instance) if you with to access the ensemble.
        /// </summary>
        Expired = -112
    }
}