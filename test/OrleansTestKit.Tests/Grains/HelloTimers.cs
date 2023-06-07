namespace TestGrains;

public class HelloTimers : Grain<HelloTimersState>, IGrainWithIntegerKey
{
    private IDisposable? _secretTimer;

    private IDisposable _timer0 = default!;

    private IDisposable _timer1 = default!;

    private IDisposable _timer2 = default!;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _timer0 = RegisterTimer(_ => OnTimer0(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _timer1 = RegisterTimer(_ => OnTimer1(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _timer2 = RegisterTimer(_ => OnTimer2(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        return base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _timer0.Dispose();
        _timer1.Dispose();

        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    public Task RegisterSecretTimer()
    {
        _secretTimer = RegisterTimer(_ => OnSecretTimer(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    private Task OnSecretTimer()
    {
        _secretTimer?.Dispose();
        _secretTimer = RegisterTimer(_ => OnSecretTimer(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    private Task OnTimer0()
    {
        State.Timer0Fired = true;
        return Task.CompletedTask;
    }

    private Task OnTimer1()
    {
        State.Timer1Fired = true;
        return Task.CompletedTask;
    }

    private Task OnTimer2()
    {
        State.Timer2Fired = true;
        _timer2.Dispose();
        _timer2 = RegisterTimer(_ => OnTimer2(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }
}

public class HelloTimersState
{
    public HelloTimersState()
    {
        Timer0Fired = false;
        Timer1Fired = false;
    }

    public bool Timer0Fired { get; set; }

    public bool Timer1Fired { get; set; }

    public bool Timer2Fired { get; set; }
}
