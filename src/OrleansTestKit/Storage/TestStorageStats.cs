namespace Orleans.TestKit.Storage;

public sealed class TestStorageStats
{
    public int Clears { get; set; }

    public int Reads { get; set; }

    public int Writes { get; set; }

    public void ResetCounts()
    {
        Writes = 0;
        Reads = 0;
        Clears = 0;
    }
}
