using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

public class ReminderContextHandler : IDisposable
{
    private bool _disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task SetActivationContext(IGrainContext context, CancellationToken token = default) =>
        _disposed
            ? throw new ObjectDisposedException(nameof(ReminderContextHandler))
            : ReminderContextHolder.Instance.SetReminderContext(context, token);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ReminderContextHolder.Instance.ReleaseReminderContext();
            }

            _disposed = true;
        }
    }
}
