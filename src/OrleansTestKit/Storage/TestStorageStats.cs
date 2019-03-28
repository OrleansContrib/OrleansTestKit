namespace Orleans.TestKit.Storage
{
    public sealed class TestStorageStats
    {
        public int Writes { get; internal set; }

        public int Clears { get; internal set; }

        public int Reads { get; internal set; }

        public void ResetCounts()
        {
            Writes = 0;
            Reads = 0;
            Clears = 0;
        }
    }
}
