using Orleans.TestKit;

namespace Orleans.TestKit
{
    public static class TimerExtensions
    {
        public static void FireTimer(this TestKitSilo silo, int index)
        {
            silo.TimerReistry.Fire(index);
        }

        public static void FireAllTimers(this TestKitSilo silo)
        {
            silo.TimerReistry.FireAll();
        }
    }
}