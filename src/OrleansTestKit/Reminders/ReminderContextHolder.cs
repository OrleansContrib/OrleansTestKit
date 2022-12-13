using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

public sealed class ReminderContextHolder
{
    private const string RUNTIME_NAMESPACE = "Orleans.Runtime.RuntimeContext";
    private const string RUNTIME_METHOD = "SetExecutionContext";
    private static readonly Lazy<ReminderContextHolder> lazy =
        new Lazy<ReminderContextHolder>(() => new ReminderContextHolder());
    private readonly SemaphoreSlim _slim;
    private readonly MethodInfo _runtimeContextMethod;

    public static ReminderContextHolder Instance { get { return lazy.Value; } }

    private ReminderContextHolder()
    {
        _slim = new SemaphoreSlim(1);
        var assembly = typeof(GrainId).Assembly;
        var contextType = assembly.GetType(RUNTIME_NAMESPACE);
        _runtimeContextMethod = contextType.GetMethod(RUNTIME_METHOD, BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(IGrainContext) });
    }

    public async Task SetReminderContext(IGrainContext context, CancellationToken token = default)
    {
        await _slim.WaitAsync(token);

        _runtimeContextMethod.Invoke(null, new object[] { context });
    }

    public void ReleaseReminderContext()
    {
        try
        {
            _runtimeContextMethod.Invoke(null, new object[] { null });
        }
        finally
        {
            _slim.Release();
        }

    }

}
