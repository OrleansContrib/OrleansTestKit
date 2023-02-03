using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;

namespace TestGrains
{
    public class HelloReminders : Grain, IGrainWithIntegerKey, IRemindable
    {
        public readonly List<string> FiredReminders = new();

        public Task RegisterReminder(string reminderName, TimeSpan dueTime, TimeSpan period)
        {
            return this.RegisterOrUpdateReminder(reminderName, dueTime, period);
        }

        Task IRemindable.ReceiveReminder(string reminderName, TickStatus status)
        {
            FiredReminders.Add(reminderName);
            return Task.CompletedTask;
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
}
