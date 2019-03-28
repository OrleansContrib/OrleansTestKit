using System.Threading.Tasks;
using Orleans.Runtime;

namespace Orleans.TestKit
{
    public static class ReminderExtensions
    {
        public static Task FireReminder(this TestKitSilo silo, string reminderName, TickStatus tickStatus = new TickStatus())
        => silo.ReminderRegistry.FireReminder(reminderName, tickStatus);

        public static Task FireAllReminders(this TestKitSilo silo, TickStatus tickStatus = new TickStatus())
         => silo.ReminderRegistry.FireAllReminders(tickStatus);
    }
}