using TestInterfaces;

namespace TestGrains;

using Orleans.Runtime;

public class DeviceIosGrain : IGrainBase, IDevice
{
    public Task<string> GetDeviceType()
    {
        return Task.FromResult("IOS");
    }

    public IGrainContext GrainContext { get; set; }
}
