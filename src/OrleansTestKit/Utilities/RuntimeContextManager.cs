using System.Reflection;
using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

internal sealed class RuntimeContextManager
{
    public delegate void SetExecutionContextDelegate(IGrainContext? newContext, out IGrainContext currentContext);

    private RuntimeContextManager()
    {
        var assembly = typeof(GrainId).Assembly;
        var contextType = assembly.GetType("Orleans.Runtime.RuntimeContext")!;

        var methodInfo = contextType
            .GetMethod("SetExecutionContext",
                BindingFlags.NonPublic | BindingFlags.Static,
                new Type[] { typeof(IGrainContext), typeof(IGrainContext).MakeByRefType() });
        SetExecutionContext = methodInfo!
                .CreateDelegate<SetExecutionContextDelegate>()
            ;
    }

    public static RuntimeContextManager Instance { get; } = new RuntimeContextManager();

    public SetExecutionContextDelegate SetExecutionContext { get; }

    public static IDisposable StartExecutionContext(IGrainContext context) => new RuntimeContextScope(context);

    public void ResetExecutionContext() => SetExecutionContext(null, out _);

    private sealed class RuntimeContextScope : IDisposable
    {
        public RuntimeContextScope(IGrainContext context) => Instance.SetExecutionContext(context, out _);

        public void Dispose() => Instance.ResetExecutionContext();
    }
}
