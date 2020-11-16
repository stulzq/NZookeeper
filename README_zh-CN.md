# NZookeeper

[English](README.md)|中文

NZookeeper 是 Zookeeper 客户端库 ZookeeperEx 的封装，旨在**简化** ZookeeperEx 的使用。

> 目前还不支持事务操作，下个版本支持

## 快速入门

1.连接 Zookeeper

````csharp
using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Debug)
                    .AddFilter("System", LogLevel.Information)
                    .AddConsole();
            });
var logger = loggerFactory.CreateLogger<ZkConnection>();
//多个Zookeeper地址使用英文逗号分隔
var zk = new ZkConnection(new ZkConnectionOptions() { ConnectionString = "localhost:2181", SessionTimeout = 5000 }, logger);
await zk.ConnectAsync();
````

2.设置 Watch 事件

> 用于监听节点变化和数据变化，节点变化包括：节点创建，节点删除

````csharp
zk.OnWatch += Zk_OnWatch;

private Task Zk_OnWatch(ZkWatchEventArgs args)
{
    Console.WriteLine($"OnWatch: Path {args.Path}, Type {args.EventType}, State {args.EventType}");
    return Task.CompletedTask;
}
````

3.节点操作

````csharp
//创建节点
await zk.CreateNodeAsync("/mynode", "节点数据",
                    new List<Acl>() { new Acl(AclPerm.All, AclScheme.World, AclId.World()) }, NodeType.Ephemeral);
//获取子节点
await zk.GetChildrenAsync("/mynode");
//删除节点
await zk.DeleteNodeAsync("/mynode");
//检查节点是否存在
await zk.NodeExistsAsync("/mynode")
````

4.数据

````csharp
//更新节点数据
await zk.SetDataAsync("/mynode", "111");
//获取节点数据
await zk.GetDataAsync("/mynode")
````

5.ACL

````csharp
//获取ACL
await zk.GetAclAsync("/mynode");
//设置ACL
await zk.SetAclAsync("/mynode",new List<Acl>() { new Acl(AclPerm.All, AclScheme.World, AclId.World()) })
````

