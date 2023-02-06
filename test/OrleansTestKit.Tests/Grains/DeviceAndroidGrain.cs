using TestInterfaces;

namespace TestGrains;

public class DeviceAndroidGrain : Grain, IDevice
{
    public Task<string> GetDeviceType()
    {
        return Task.FromResult("Android");
    }
}
