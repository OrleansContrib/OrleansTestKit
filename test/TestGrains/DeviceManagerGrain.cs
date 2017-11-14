using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class DeviceManagerGrain : Grain, IDeviceManager
    {
        public Task<IDevice> GetDeviceGrain(string deviceType)
        {
            switch (deviceType)
            {
                case "IOS":
                    return Task.FromResult(GrainFactory.GetGrain<IDevice>(deviceType, "TestGrains.DeviceIosGrain"));
                case "Android":
                    return Task.FromResult(GrainFactory.GetGrain<IDevice>(deviceType, "TestGrains.DeviceAndroidGrain"));
                default:
                    throw new Exception($"Unknown device type {deviceType}");
            }
        }
    }
}
