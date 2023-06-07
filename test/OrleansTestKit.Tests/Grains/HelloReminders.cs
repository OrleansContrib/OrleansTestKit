using Orleans.Runtime;

namespace TestGrains;

public sealed class HelloReminders : Grain, IGrainWithIntegerKey, IRemindable
{
    public List<string> FiredReminders { get; } = new();

    Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
    {
        FiredReminders.Add(reminderName);
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
