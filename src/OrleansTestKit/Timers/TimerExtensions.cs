using System;
using System.Threading.Tasks;

namespace Orleans.TestKit
{
    public static class TimerExtensions
    {
        [Obsolete("Use FireAllTimersAsync instead.")]
        public static void FireAllTimers(this TestKitSilo silo)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.TimerRegistry.FireAll();
        }

        public static Task FireAllTimersAsync(this TestKitSilo silo)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.TimerRegistry.FireAllAsync();
        }

        [Obsolete("Use FireTimerAsync instead.")]
        public static void FireTimer(this TestKitSilo silo, int index)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.TimerRegistry.Fire(index);
        }

        public static Task FireTimerAsync(this TestKitSilo silo, int index)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            return silo.TimerRegistry.FireAsync(index);
        }
    }
}
