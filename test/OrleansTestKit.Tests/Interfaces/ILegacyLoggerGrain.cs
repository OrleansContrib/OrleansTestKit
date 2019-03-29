using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface ILegacyLoggerGrain : IGrainWithIntegerKey
    {
        Task WriteToCustomLog(string message);

        Task WriteToDefaultLog(string message);
    }
}
