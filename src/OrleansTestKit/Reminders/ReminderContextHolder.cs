using System.Reflection;
using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

public sealed class ReminderContextHolder : IDisposable
{
    private const string RuntimeMethod = "SetExecutionContext";

    private const string RuntimeNamespace = "Orleans.Runtime.RuntimeContext";

    private static readonly Lazy<ReminderContextHolder> Lazy = new(() => new ReminderContextHolder());

    private readonly MethodInfo _runtimeContextMethod;

    private readonly SemaphoreSlim _slim;

    private ReminderContextHolder()
    {
        _slim = new SemaphoreSlim(1);
        var assembly = typeof(GrainId).Assembly;
        var contextType = assembly.GetType(RuntimeNamespace)!;
        _runtimeContextMethod = contextType.GetMethod(RuntimeMethod, BindingFlags.NonPublic | BindingFlags.Static)!;
    }

    public static ReminderContextHolder Instance =>
        Lazy.Value;

    public void Dispose() =>
        _slim.Dispose();

    public void ReleaseReminderContext()
    {
        try
        {
            _runtimeContextMethod.Invoke(null, new object?[] { null, null });
        }
        finally
        {
            _slim.Release();
        }
    }

    public async Task SetReminderContext(IGrainContext context, CancellationToken token = default)
    {
        await _slim.WaitAsync(token);
        _runtimeContextMethod.Invoke(null, new object?[] { context, null });
    }
}
