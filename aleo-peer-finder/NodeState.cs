//{
//    "jsonrpc": "2.0",
//    "result": {
//        "candidate_peers": [
//        "135.181.206.65:4132",
//        "159.223.124.150:4132",
//        "144.126.219.193:4132"
//            ],
//        "connected_peers": [
//        "143.198.166.150:4132",
//        "147.139.105.12:4132",
//        "46.148.231.72:4132",
//        "167.99.40.226:4132",
//        "165.232.145.194:4132",
//        "161.35.106.91:4132",
//        "195.201.197.79:4132",
//        "159.223.118.35:4132",
//        "188.166.7.13:4132",
//        "103.244.117.26:4132",
//        "147.182.213.228:4132",
//        "37.230.117.126:4132",
//        "157.245.133.62:4132",
//        "156.146.34.26:443",
//        "137.184.192.155:4132",
//        "137.184.202.162:4132"
//            ],
//        "latest_block_height": 8825,
//        "latest_cumulative_weight": 12653,
//        "number_of_candidate_peers": 3,
//        "number_of_connected_peers": 16,
//        "number_of_connected_sync_nodes": 10,
//        "software": "snarkOS 2.0.0",
//        "status": "Syncing",
//        "type": "Miner",
//        "version": 11
//    },
//    "id": "1"
//}

using System.Text.Json.Serialization;

namespace AleoPeerFinder
{
    internal class NodeState
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonPropertyName("result")]
        public NodeStateResult Result { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    internal class NodeStateResult
    {
        [JsonPropertyName("candidate_peers")]
        public List<string> CandidatePeers { get; set; }

        [JsonPropertyName("connected_peers")]
        public List<string> ConnectedPeers { get; set; }

        [JsonPropertyName("latest_block_height")]
        public int LatestBlockHeight { get; set; }

        [JsonPropertyName("latest_cumulative_weight")]
        public int LatestCumulativeWeight { get; set; }

        [JsonPropertyName("number_of_candidate_peers")]
        public int NumberOfCandidatePeers { get; set; }

        [JsonPropertyName("number_of_connected_peers")]
        public int NumberOfConnectedPeers { get; set; }

        [JsonPropertyName("number_of_connected_sync_nodes")]
        public int NumberOfConnectedSyncNodes { get; set; }

        [JsonPropertyName("software")]
        public string Software { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }
    }
}
