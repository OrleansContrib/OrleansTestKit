using Orleans.Runtime;
using Orleans.Streams;

namespace TestGrains;

public sealed class ActivationCountWithReminder : Grain, IGrainWithIntegerKey, IRemindable
{
    private int _activationCount;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _activationCount++;
        return base.OnActivateAsync(cancellationToken);
    }

    public Task<int> GetActivationCount() => Task.FromResult(_activationCount);


    Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
    {
        return Task.CompletedTask;
    }

    public Task RegisterReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
    {
        return this.RegisterOrUpdateReminder(reminderName, dueTime, period);
    }

    public async Task UnregisterReminder(string reminderName)
    {
        var reminder = await this.GetReminder(reminderName);

        if (reminder != null)
        {
            await this.UnregisterReminder(reminder);
        }
    }
}
