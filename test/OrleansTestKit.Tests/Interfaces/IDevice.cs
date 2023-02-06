namespace TestInterfaces;

public interface IDevice : IGrainWithStringKey
{
    Task<string> GetDeviceType();
}
