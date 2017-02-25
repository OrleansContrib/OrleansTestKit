namespace Orleans.TestKit.Storage
{
    public sealed class TestStorageStats
    {
        public uint Writes { get; internal set; }

        public uint Clears { get; internal set; }

        public uint Reads { get; internal set; }
    }
}