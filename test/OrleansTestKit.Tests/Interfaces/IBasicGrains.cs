using System;
using System.Threading.Tasks;
using Orleans;

namespace TestInterfaces
{
    public interface IGuidKeyGrain : IGrainWithGuidKey
    {
        Task<Guid> GetKey();
    }

    public interface IGuidCompoundKeyGrain : IGrainWithGuidCompoundKey
    {
        Task<(Guid, string)> GetKey();
    }

    public interface IIntegerCompoundKeyGrain : IGrainWithIntegerCompoundKey
    {
        Task<(long, string)> GetKey();
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
