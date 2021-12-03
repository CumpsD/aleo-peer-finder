namespace AleoPeerFinder
{
    internal class NodeDetails
    {
        public string Ip { get; }
        public int? Block { get; set; }
        public int? Weight { get; set; }

        public NodeDetails(string ip)
        {
            Ip = ip;
        }

        public NodeDetails(string ip, int block, int weight) : this(ip)
        {
            Block = block;
            Weight = weight;
        }
    }
}
