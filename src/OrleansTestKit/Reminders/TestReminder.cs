using Orleans.Runtime;

namespace Orleans.TestKit.Reminders;

public sealed class TestReminder :
    IGrainReminder
{
    public TestReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
    {
        ReminderName = reminderName ?? throw new ArgumentNullException(nameof(reminderName));
        DueTime = dueTime;
        Period = period;
    }

    public TimeSpan DueTime { get; }

    public TimeSpan Period { get; }

    public string ReminderName { get; }
}
