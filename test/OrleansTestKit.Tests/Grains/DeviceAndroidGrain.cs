using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public class DeviceAndroidGrain : Grain, IDevice
    {
        public Task<string> GetDeviceType()
        {
            return Task.FromResult("Android");
        }
    }
}
