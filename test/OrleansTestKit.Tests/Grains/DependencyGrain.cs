using Microsoft.Extensions.DependencyInjection;
using TestInterfaces;

namespace TestGrains;

public interface IDependency
{
    string? GetValue();
}

public sealed class DependencyGrain : Grain, IDependencyGrain
{
    private readonly IDependency? _firstKeyedDependency;

    private readonly IDependency? _secondKeyedDependency;

    private readonly IDependency? _unkeyedDependency;

    public DependencyGrain(
        IDependency? unkeyedDependency,
        [FromKeyedServices("first")] IDependency? firstKeyedDependency,
        [FromKeyedServices("second")] IDependency? secondKeyedDependency)
    {
        _unkeyedDependency = unkeyedDependency;
        _firstKeyedDependency = firstKeyedDependency;
        _secondKeyedDependency = secondKeyedDependency;
    }

    public Task<string?> GetFirstKeyedServiceValue() =>
        Task.FromResult(_firstKeyedDependency?.GetValue());

    public Task<string?> GetSecondKeyedServiceValue() =>
        Task.FromResult(_secondKeyedDependency?.GetValue());

    public Task<string?> GetUnkeyedServiceValue() =>
        Task.FromResult(_unkeyedDependency?.GetValue());
}
