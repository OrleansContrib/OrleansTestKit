using TestInterfaces;

namespace TestGrains;

using Orleans.Runtime;

public class DeviceAndroidGrain : IGrainBase, IDevice
{
    public Task<string> GetDeviceType()
    {
        return Task.FromResult("Android");
    }

    public IGrainContext GrainContext { get; set; }
}
