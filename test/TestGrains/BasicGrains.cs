using System;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class GuidKeyGrain : Grain, IGuidKeyGrain
    {
        public Task<Guid> GetKey() => Task.FromResult(this.GetPrimaryKey());
    }

    public sealed class IntegerKeyGrain : Grain, IIntegerKeyGrain
    {
        public Task<long> GetKey() => Task.FromResult(this.GetPrimaryKeyLong());
    }

    public sealed class StringKeyGrain : Grain, IStringKeyGrain
    {
        public Task<string> GetKey() => Task.FromResult(this.GetPrimaryKeyString());
    }

    public sealed class StatefulGuidKeyGrain : Grain<object>, IGuidKeyGrain
    {
        public Task<Guid> GetKey() => Task.FromResult(this.GetPrimaryKey());
    }

    public sealed class StatefulIntegerKeyGrain : Grain<object>, IIntegerKeyGrain
    {
        public Task<long> GetKey() => Task.FromResult(this.GetPrimaryKeyLong());
    }

    public sealed class StatefulStringKeyGrain : Grain<object>, IStringKeyGrain
    {
        public Task<string> GetKey() => Task.FromResult(this.GetPrimaryKeyString());
    }
}