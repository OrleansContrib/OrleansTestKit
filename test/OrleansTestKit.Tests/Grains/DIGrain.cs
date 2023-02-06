namespace TestGrains;

public interface IDIService
{
    bool GetValue();
}

public sealed class DIGrain : Grain, IGrainWithGuidKey
{
    public DIGrain(IDIService service)
    {
        Service = service;
    }

    public IDIService Service { get; }

    public bool GetServiceValue() => Service.GetValue();
}
