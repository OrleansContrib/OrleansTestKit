namespace TestInterfaces;

public interface IDeviceManager : IGrainWithIntegerKey
{
    Task<IDevice> GetDeviceGrain(string deviceType);

    Task<string> GetDeviceType(string deviceType);
}
