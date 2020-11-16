# NZookeeper
[![Latest version](https://img.shields.io/nuget/v/CanalSharp.svg)](https://www.nuget.org/packages/CanalSharp/) 

English|[中文](README_zh-CN.md)

A zookeeper client library based on ZookeeperEx，easily use for Zookeeper.

> Transaction operation is not supported at present, and will be supported in the next version

## Get start

1.Connect to Zookeeper

````csharp
using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Debug)
                    .AddFilter("System", LogLevel.Information)
                    .AddConsole();
            });
var logger = loggerFactory.CreateLogger<ZkConnection>();
//Multiple zookeeper addresses are separated by ,
var zk = new ZkConnection(new ZkConnectionOptions() { ConnectionString = "localhost:2181", SessionTimeout = 5000 }, logger);
await zk.ConnectAsync();
````

2.Set watch event

> It is used to monitor node changes and data changes. Node changes include: node creation and node deletion

````csharp
zk.OnWatch += Zk_OnWatch;

private Task Zk_OnWatch(ZkWatchEventArgs args)
{
    Console.WriteLine($"OnWatch: Path {args.Path}, Type {args.EventType}, State {args.State}");
    return Task.CompletedTask;
}
````

3.Node operation

````csharp
//Create node
await zk.CreateNodeAsync("/mynode", "nodedata",
                    new List<Acl>() { new Acl(AclPerm.All, AclScheme.World, AclId.World()) }, NodeType.Ephemeral);
//Get child node
await zk.GetChildrenAsync("/mynode");
//Delete node
await zk.DeleteNodeAsync("/mynode");
//Check if the node exists
await zk.NodeExistsAsync("/mynode")
````

4.Data

````csharp
//Update node data
await zk.SetDataAsync("/mynode", "111");
//Get node data
await zk.GetDataAsync("/mynode")
````

5.ACL

````csharp
//Get ACL
await zk.GetAclAsync("/mynode");
//Set ACL
await zk.SetAclAsync("/mynode",new List<Acl>() { new Acl(AclPerm.All, AclScheme.World, AclId.World()) })
````

