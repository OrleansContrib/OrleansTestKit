using System.Reflection;
using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

internal sealed class RuntimeContextManager
{
    private RuntimeContextManager()
    {
        var assembly = typeof(GrainId).Assembly;
        var contextType = assembly.GetType("Orleans.Runtime.RuntimeContext")!;

        SetExecutionContext = contextType
            .GetMethod("SetExecutionContext", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(IGrainContext), typeof(IGrainContext) })!
            .CreateDelegate<Action<IGrainContext?>>()
        ;
    }

    public static RuntimeContextManager Instance { get; } = new RuntimeContextManager();

    public Action<IGrainContext?> SetExecutionContext { get; }

    public static IDisposable StartExecutionContext(IGrainContext context) => new RuntimeContextScope(context);

    public void ResetExecutionContext() => SetExecutionContext(null);

    private sealed class RuntimeContextScope : IDisposable
    {
        public RuntimeContextScope(IGrainContext context) => Instance.SetExecutionContext(context);

        public void Dispose() => Instance.ResetExecutionContext();
    }
}
