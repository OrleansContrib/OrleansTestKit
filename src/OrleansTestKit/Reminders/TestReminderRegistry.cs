﻿using Moq;
using Orleans.Runtime;
using Orleans.Timers;

namespace Orleans.TestKit.Reminders;

public sealed class TestReminderRegistry : IReminderRegistry
{
    private readonly Dictionary<string, TestReminder> _reminders = new();

    private IRemindable? _grain;

    private IRemindable Grain => _grain ?? throw new InvalidOperationException($"You must SetGrainTarget before invoking other methods");

    public Mock<IReminderRegistry> Mock { get; } = new();

    public async Task FireAllReminders(TickStatus tickStatus)
    {
        foreach (var reminderName in _reminders.Keys)
        {
            await Grain.ReceiveReminder(reminderName, tickStatus);
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

        return Grain.ReceiveReminder(reminderName, tickStatus);
    }

    public async Task<IGrainReminder?> GetReminder(GrainId callingGrainId, string reminderName)
    {
        ArgumentNullException.ThrowIfNull(reminderName);

        await Mock.Object.GetReminder(callingGrainId, reminderName);
        return !_reminders.TryGetValue(reminderName, out var reminder) ? null : reminder;
    }

    public async Task<List<IGrainReminder>> GetReminders(GrainId callingGrainId)
    {
        await Mock.Object.GetReminders(callingGrainId);
        return _reminders.Values.ToList<IGrainReminder>();
    }

    public async Task<IGrainReminder> RegisterOrUpdateReminder(GrainId callingGrainId, string reminderName, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == null)
        {
            throw new ArgumentNullException(nameof(reminderName));
        }

        await Mock.Object.RegisterOrUpdateReminder(callingGrainId, reminderName, dueTime, period);
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

        await Mock.Object.UnregisterReminder(callingGrainId, reminder);
        _reminders.Remove(reminder.ReminderName);
    }

    internal void SetGrainTarget(IRemindable grain) =>
        _grain = grain ?? throw new ArgumentNullException(nameof(grain));
}
