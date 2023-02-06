namespace Orleans.TestKit;

public static class TimerExtensions
{
    public static Task FireAllTimersAsync(this TestKitSilo silo)
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        return silo.TimerRegistry.FireAllAsync();
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
