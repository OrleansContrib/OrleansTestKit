namespace TestInterfaces;

public interface IDependencyGrain : IGrainWithGuidKey
{
    Task<string?> GetFirstKeyedServiceValue();

    Task<string?> GetSecondKeyedServiceValue();

    Task<string?> GetUnkeyedServiceValue();
}
