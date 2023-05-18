using Orleans.Runtime;

namespace Orleans.TestKit;

/// <summary>
/// Extensions for test kit reminder firing.
/// </summary>
public static class ReminderExtensions
{
    /// <summary>
    /// Fire all reminders for this silo.
    /// </summary>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="tickStatus">The tick status.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static Task FireAllReminders(this TestKitSilo silo, TickStatus tickStatus = default)
    {
        ArgumentNullException.ThrowIfNull(silo);

        return silo.ReminderRegistry.FireAllReminders(tickStatus);
    }

    /// <summary>
    /// Fire specific reminder for this silo.
    /// </summary>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="reminderName">The reminder to fire.</param>
    /// <param name="tickStatus">The tick status.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static Task FireReminder(this TestKitSilo silo, string reminderName, TickStatus tickStatus = default)
    {
        ArgumentNullException.ThrowIfNull(silo);
        ArgumentNullException.ThrowIfNull(reminderName);

        return silo.ReminderRegistry.FireReminder(reminderName, tickStatus);
    }
}
