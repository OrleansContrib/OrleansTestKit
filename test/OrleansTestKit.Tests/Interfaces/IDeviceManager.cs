using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IDeviceManager : IGrainWithIntegerKey
    {
        Task<IDevice> GetDeviceGrain(string deviceType);

        Task<string> GetDeviceType(string deviceType);
    }
}
