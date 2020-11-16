using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NZookeeper.ACL;
using NZookeeper.Node;

namespace NZookeeper.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Debug)
                    .AddFilter("System", LogLevel.Information)
                    .AddConsole();
            });
            var logger = loggerFactory.CreateLogger<ZkConnection>();
            var zk = new ZkConnection(new ZkConnectionOptions() { ConnectionString = "localhost:2181", SessionTimeout = 5000 }, logger);
            zk.OnWatch += Zk_OnWatch;
            await zk.ConnectAsync();
            while (true)
            {
                await zk.CreateNodeAsync("/mynode", "ab",
                    new List<Acl>() { new Acl(AclPerm.All, AclScheme.World, AclId.World()) }, NodeType.Ephemeral);
                await zk.SetDataAsync("/mynode", "111");
                await zk.GetChildrenAsync("/mynode");
                await Task.Delay(1000);
                await zk.DeleteNodeAsync("/mynode");
                
            }
        }

        private static Task Zk_OnWatch(ZkWatchEventArgs args)
        {
            Console.WriteLine($"OnWatch: Path {args.Path}, Type {args.EventType}, State {args.EventType}");
            return Task.CompletedTask;
        }
    }
}
