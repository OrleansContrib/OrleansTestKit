#if NSUBSTITUTE

using NSubstitute;

#else
using Moq;
#endif

using Orleans.Runtime;
using Orleans.Timers;

namespace Orleans.TestKit.Reminders;

public sealed class TestReminderRegistry : IReminderRegistry
{
    private readonly Dictionary<string, TestReminder> _reminders = new();

    private IRemindable _grain;

#if NSUBSTITUTE

    public IReminderRegistry Mock { get; } = Substitute.For<IReminderRegistry>();

#else
    public Mock<IReminderRegistry> Mock { get; } = new();
#endif

    public async Task FireAllReminders(TickStatus tickStatus)
    {
        foreach (var reminderName in _reminders.Keys)
        {
            await _grain.ReceiveReminder(reminderName, tickStatus);
        }
    }

    public Task FireReminder(string reminderName, TickStatus tickStatus)
    {
        if (reminderName == null)
        {
            throw new ArgumentNullException(nameof(reminderName));
        }

        if (!_reminders.ContainsKey(reminderName))
        {
            throw new ArgumentException($"No reminder named {reminderName} found");
        }

        return _grain.ReceiveReminder(reminderName, tickStatus);
    }

    public async Task<IGrainReminder> GetReminder(GrainId callingGrainId, string reminderName)
    {
        if (reminderName == null)
        {
            throw new ArgumentNullException(nameof(reminderName));
        }

#if NSUBSTITUTE
        await Mock.GetReminder(callingGrainId, reminderName);
#else
        await Mock.Object.GetReminder(callingGrainId, reminderName);
#endif
        return !_reminders.TryGetValue(reminderName, out var reminder) ? null : reminder;
    }

    public async Task<List<IGrainReminder>> GetReminders(GrainId callingGrainId)
    {
#if NSUBSTITUTE
        await Mock.GetReminders(callingGrainId);
#else
        await Mock.Object.GetReminders(callingGrainId);
#endif

        return _reminders.Values.ToList<IGrainReminder>();
    }

    public async Task<IGrainReminder> RegisterOrUpdateReminder(GrainId callingGrainId, string reminderName, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == null)
        {
            throw new ArgumentNullException(nameof(reminderName));
        }
#if NSUBSTITUTE
        await Mock.RegisterOrUpdateReminder(callingGrainId, reminderName, dueTime, period);
#else
        await Mock.Object.RegisterOrUpdateReminder(callingGrainId, reminderName, dueTime, period);
#endif
        var reminder = new TestReminder(reminderName, dueTime, period);
        _reminders[reminderName] = reminder;
        return reminder;
    }

    public async Task UnregisterReminder(GrainId callingGrainId, IGrainReminder reminder)
    {
        if (reminder == null)
        {
            throw new ArgumentNullException(nameof(reminder));
        }

#if NSUBSTITUTE
        await Mock.UnregisterReminder(callingGrainId, reminder);
#else
        await Mock.Object.UnregisterReminder(callingGrainId, reminder);
#endif
        _reminders.Remove(reminder.ReminderName);
    }

    internal void SetGrainTarget(IRemindable grain) =>
        _grain = grain ?? throw new ArgumentNullException(nameof(grain));
}
