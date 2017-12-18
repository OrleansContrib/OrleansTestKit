using System;
using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IGuidKeyGrain : IGrainWithGuidKey
    {
        Task<Guid> GetKey();
    }

    public interface IIntegerKeyGrain : IGrainWithIntegerKey
    {
        Task<long> GetKey();
    }

    public interface IStringKeyGrain : IGrainWithStringKey
    {
        Task<string> GetKey();
    }
}