using TestInterfaces;

namespace TestGrains;

using Orleans.Runtime;

public abstract class DerivedGrain : IGrainBase
{
    public IGrainContext GrainContext { get; }
}

public class DeviceManagerGrain : DerivedGrain, IDeviceManager
{
    private readonly IGrainFactory _grainFactory;

    public DeviceManagerGrain(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public Task<IDevice> GetDeviceGrain(string deviceType)
    {
        switch (deviceType)
        {
            case "IOS":
                return Task.FromResult(_grainFactory.GetGrain<IDevice>(deviceType, "TestGrains.DeviceIosGrain"));

            case "Android":
                return Task.FromResult(_grainFactory.GetGrain<IDevice>(deviceType, "TestGrains.DeviceAndroidGrain"));

            default:
                throw new InvalidOperationException($"Unknown device type {deviceType}");
        }
    }

    public async Task<string> GetDeviceType(string deviceType)
    {
        var device = await this.GetDeviceGrain(deviceType);
        return await device.GetDeviceType();
    }

    //public IGrainContext GrainContext { get; set; }
}
