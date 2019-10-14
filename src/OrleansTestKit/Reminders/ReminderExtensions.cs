using System;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public static class ReminderExtensions
    {
        public static Task FireReminder(this TestKitSilo silo, string reminderName, TickStatus tickStatus = default)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            if (reminderName == null)
            {
                throw new ArgumentNullException(nameof(reminderName));
            }

            return silo.ReminderRegistry.FireReminder(reminderName, tickStatus);
        }

        public static Task FireAllReminders(this TestKitSilo silo, TickStatus tickStatus = default)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.ReminderRegistry.FireAllReminders(tickStatus);
        }
    }
}
