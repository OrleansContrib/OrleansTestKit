using TestInterfaces;

namespace TestGrains;

public class DeviceIosGrain : Grain, IDevice
{
    public Task<string> GetDeviceType()
    {
        return Task.FromResult("IOS");
    }
}
