using System;

namespace Orleans.TestKit
{
    public static class TimerExtensions
    {
        public static void FireAllTimers(this TestKitSilo silo)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.TimerRegistry.FireAll();
        }

        public static void FireTimer(this TestKitSilo silo, int index)
        {
            if (silo == null)
            {
                throw new ArgumentNullException(nameof(silo));
            }

            silo.TimerRegistry.Fire(index);
        }
    }
}
