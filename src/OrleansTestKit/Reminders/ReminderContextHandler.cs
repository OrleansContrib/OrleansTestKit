using System;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;
public class ReminderContextHandler : IDisposable
{
    private bool disposedValue;


    public Task SetActivationContext(IGrainContext context, CancellationToken token = default) =>
        ReminderContextHolder.Instance.SetReminderContext(context, token);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ReminderContextHolder.Instance.ReleaseReminderContext();
            }

            disposedValue = true;
        }
    }


    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
