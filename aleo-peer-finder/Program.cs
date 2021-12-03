using System.Collections.Concurrent;
using System.Net.Http.Json;
using Microsoft.Net.Http.Headers;

namespace AleoPeerFinder
{
    public static class Program
    {
        private const int MaxNumberOfLoops = 10;
        private const int NodesToTake = 50;

        private static readonly HttpClient Client = new();

        private const string RpcBody = "{ \"jsonrpc\": \"2.0\", \"id\": \"1\", \"method\": \"getnodestate\", \"params\": [] }";

        private static readonly List<string> SyncNodes = new()
        {
            "144.126.219.193", "165.232.145.194", "143.198.164.241", "188.166.7.13", "167.99.40.226",
            "159.223.124.150", "137.184.192.155", "147.182.213.228", "137.184.202.162", "159.223.118.35",
            "161.35.106.91", "157.245.133.62", "143.198.166.150"
        };

        private static readonly ConcurrentDictionary<string, NodeDetails> Nodes = new();

        private static int _fetchCount;

        public static async Task Main()
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "Aleo Peer Finder");
            Client.Timeout = TimeSpan.FromMilliseconds(1000);

            Console.WriteLine($"Processing {SyncNodes.Count} sync nodes.");
            await ProcessNodes(SyncNodes);

            var numberOfLoops = 0;
            while (numberOfLoops < MaxNumberOfLoops && Nodes.Values.Any(x => x.Block == null))
            {
                var newNodes = Nodes
                    .Where(x => x.Value.Block == null)
                    .Select(x => x.Key)
                    .ToList();

                Console.WriteLine($"Processing {newNodes.Count} new nodes.");

                await ProcessNodes(newNodes);
                numberOfLoops++;
            }

            var nodes = Nodes
                .Where(x => x.Value.Block != null)
                .OrderByDescending(x => x.Value.Weight)
                .ToList();

            Console.WriteLine();
            Console.WriteLine($"Found {nodes.Count} nodes.");
            Console.WriteLine();

            foreach (var (_, node) in nodes)
                Console.WriteLine($"{node.Ip,22} - {node.Block,5} / {node.Weight,5}");

            Console.WriteLine("Paste this in environment/mod.rs");
            Console.WriteLine();

            var highestNodes = string.Join(", ", nodes.Take(NodesToTake).Select(x => $"\"{x.Value.Ip}\""));

            Console.WriteLine($"    const SYNC_NODES: [&'static str; {NodesToTake}] = [");
            Console.WriteLine($"      {highestNodes}");
            Console.WriteLine("    ];");
        }

        private static async Task ProcessNodes(IEnumerable<string> nodes)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 4
            };

            await Parallel.ForEachAsync(
                nodes,
                parallelOptions,
                async (node, ct) =>
                {
                    var fetchCount = Interlocked.Increment(ref _fetchCount);

                    try
                    {

                        Console.WriteLine($"#{fetchCount,5} | Fetching info for {node}");

                        var rpcCall = await Client.PostAsync($"http://{node}:3032", new StringContent(RpcBody), ct);
                        var rpcResponse = await rpcCall.Content.ReadFromJsonAsync<NodeState>(cancellationToken: ct);

                        if (rpcResponse != null)
                            ProcessNode(fetchCount, node, rpcResponse.Result);
                    }
                    catch (TimeoutException) {}
                    catch (Exception e)
                    {
                        if (e.InnerException is TimeoutException)
                            return;

                        Console.WriteLine($"#{fetchCount,5} | Fetching info for {node} failed: {e.Message}");

                        Nodes.TryRemove(node, out _);
                    }
                });
        }

        private static void ProcessNode(int fetchCount, string node, NodeStateResult result)
        {
            var currentHigh = Nodes.Max(x => x.Value.Weight);
            if (result.LatestCumulativeWeight > currentHigh)
                Console.WriteLine($"#{fetchCount,5} | New highest weight found! {result.LatestCumulativeWeight} for {node}");

            Nodes.AddOrUpdate(
                node,
                new NodeDetails(node, result.LatestBlockHeight, result.LatestCumulativeWeight),
                (_, value) =>
                {
                    value.Block = result.LatestBlockHeight;
                    value.Weight = result.LatestCumulativeWeight;
                    return value;
                });

            ProcessPeers(result.CandidatePeers);
            ProcessPeers(result.ConnectedPeers);
        }

        private static void ProcessPeers(IEnumerable<string> peers)
        {
            foreach (var peer in peers)
                Nodes.TryAdd(peer.Split(":")[0], new NodeDetails(peer));
        }
    }
}