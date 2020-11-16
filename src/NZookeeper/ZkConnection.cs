using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NZookeeper.ACL;
using NZookeeper.Internal;
using NZookeeper.Node;
using org.apache.zookeeper;
using org.apache.zookeeper.data;

namespace NZookeeper
{
    public class ZkConnection : IAsyncDisposable
    {
        private readonly ZkConnectionOptions _options;
        private readonly ILogger<ZkConnection> _logger;
        private readonly MsgPackSerializer _serializer;
        private bool _connected;
        private ZooKeeper _zk;
        // private Transaction _tran;

        public delegate Task WatchHandle(ZkWatchEventArgs args);

        public event WatchHandle OnWatch;
        public ZkConnection(ZkConnectionOptions options, ILogger<ZkConnection> logger)
        {
            _options = options;
            _logger = logger;
            _serializer = new MsgPackSerializer();
        }


        #region Connection

        public bool Connected => _connected;

        public async Task ConnectAsync()
        {
            var times = 0;
            if (OnWatch == null)
            {
                OnWatch += ZkConnection_OnWatch;
            }
            _zk = new ZooKeeper(_options.ConnectionString, _options.SessionTimeout, new InternalZkWatchWrapper(OnWatch));
            while (_zk.getState() == ZooKeeper.States.CONNECTING)
            {
                await Task.Delay(100);
                times++;
                if (times == 100)
                {
                    throw new ZkException("Connection timed out.");
                }
            }

            var state = _zk.getState();
            if (state != ZooKeeper.States.CONNECTED && state != ZooKeeper.States.CONNECTEDREADONLY)
            {
                throw new ZkException($"Connect failed, state: {state}");
            }

            _logger.LogInformation($"Zookeeper connected successfully, state: {state}");

            _connected = true;
        }

        private Task ZkConnection_OnWatch(ZkWatchEventArgs args)
        {
            return Task.CompletedTask;
        }

        public long GetSessionId()
        {
            ValidateState();
            return _zk.getSessionId();
        }

        public byte[] GetSessionPassword()
        {
            ValidateState();
            return _zk.getSessionPasswd();
        }


        public void AddAuthInfo(AclScheme scheme,byte[] auth)
        {
            ValidateState();
            _zk.addAuthInfo(scheme.ToString().ToLower(),auth);
        }

        public Task Sync([NotNull]string path)
        {
            ValidateState();
            return _zk.sync(path);
        }

        

        public async Task CloseAsync()
        {
            if (_zk != null)
            {
                await _zk.closeAsync();
                _connected = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
        }

        #endregion

        #region Data

        public async Task<NodeStatus> SetDataAsync([NotNull] string path, byte[] data, int version = -1)
        {
            ValidateState();
            var stat = await _zk.setDataAsync(path, data, version);
            await NodeExistsAsync(path);
            return new NodeStatus(stat);
        }

        public async Task<NodeStatus> SetDataAsync<T>([NotNull] string path, T data, int version = -1)
        {
            return await SetDataAsync(path, _serializer.Serialize(data), version);
        }

        public async Task<byte[]> GetDataAsync([NotNull] string path)
        {
            ValidateState();
            var dataResult = await _zk.getDataAsync(path, true);
            return dataResult.Data;
        }

        public async Task<T> GetDataAsync<T>([NotNull] string path)
        {
            ValidateState();
            var dataResult = await _zk.getDataAsync(path, true);
            return _serializer.Deserialize<T>(dataResult.Data);
        }

        #endregion

        #region Node

        [CanBeNull]
        public async Task<NodeStatus> GetNodeStatusAsync([NotNull] string path)
        {
            ValidateState();
            var stat = await _zk.existsAsync(path, true);
            return stat == null ? null : new NodeStatus(stat);
        }

        public async Task<bool> NodeExistsAsync([NotNull] string path)
        {
            ValidateState();
            var stat = await _zk.existsAsync(path, true);
            return stat != null;
        }

        public async Task CreateNodeAsync([NotNull] string path, byte[] data, List<Acl> acl, NodeType type)
        {
            ValidateState();
            CreateMode innerNodeType;

            switch (type)
            {
                case NodeType.Persistent:
                    innerNodeType = CreateMode.PERSISTENT;
                    break;
                case NodeType.PersistentSequential:
                    innerNodeType = CreateMode.PERSISTENT_SEQUENTIAL;
                    break;
                case NodeType.Ephemeral:
                    innerNodeType = CreateMode.EPHEMERAL;
                    break;
                case NodeType.EphemeralSequential:
                    innerNodeType = CreateMode.EPHEMERAL_SEQUENTIAL;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            await _zk.createAsync(path, data, ConvertToInnerAcl(acl), innerNodeType);
            await _zk.existsAsync(path, true);
        }

        public async Task CreateNodeAsync<T>([NotNull] string path, T data, List<Acl> acl, NodeType type)
        {
            ValidateState();
            await CreateNodeAsync(path, _serializer.Serialize(data), acl, type);
        }

        public async Task DeleteNodeAsync([NotNull] string path, int version = -1)
        {
            ValidateState();
            await _zk.deleteAsync(path, version);
            await NodeExistsAsync(path);
        }

        public async Task<List<string>> GetChildrenAsync([NotNull] string path)
        {
            ValidateState();
            var childrenResult = await _zk.getChildrenAsync(path, true);
            return childrenResult.Children;
        }

        #endregion

        #region ACL

        public async Task<List<Acl>> GetAclAsync([NotNull] string path)
        {
            ValidateState();
            var ackResult = await _zk.getACLAsync(path);
            return ConvertToAcl(ackResult.Acls);
        }

        public async Task SetAclAsync([NotNull] string path, [NotNull] List<Acl> acl, int version = -1)
        {
            ValidateState();
            await _zk.setACLAsync(path, ConvertToInnerAcl(acl), version);
        }

        #endregion

        // public void BeginTransaction()
        // {
        //     ValidateState();
        //     _tran = _zk.transaction();
        // }
        //
        // public void CommitTransaction()
        // {
        //     ValidateState();
        //     _tran?.commitAsync();
        // }
        //
        // public void Check([NotNull] string path,int version)
        // {
        //     ValidateState();
        //     _tran = _tran?.check(path, version);
        // }

        #region PrivateMethod

        private void ValidateState()
        {
            if (!_connected)
            {
                throw new ZkException("Zookeeper not connected.");
            }
        }

        private List<Acl> ConvertToAcl(List<org.apache.zookeeper.data.ACL> innerAcl)
        {
            var result = new List<Acl>();
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var resultAcl in innerAcl)
            {
                var resultId = resultAcl.getId();
                var acl = new Acl((AclPerm)resultAcl.getPerms(),
                    (AclScheme)Enum.Parse(typeof(AclScheme), resultId.getScheme().ToUpper(), true),
                    new AclId(resultId.getId())
                );
                result.Add(acl);
            }
            return result;
        }

        private List<org.apache.zookeeper.data.ACL> ConvertToInnerAcl(List<Acl> inputAcl)
        {
            var result = new List<org.apache.zookeeper.data.ACL>();
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var aclItem in inputAcl)
            {
                var acl = new org.apache.zookeeper.data.ACL((int)aclItem.Perm, new Id(aclItem.Scheme.ToString().ToLower(), aclItem.Id.Value));

                result.Add(acl);
            }
            return result;
        }
        #endregion

    }
}