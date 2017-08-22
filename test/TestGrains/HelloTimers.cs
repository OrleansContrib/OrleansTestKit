using System;
using System.Threading.Tasks;
using Orleans;

namespace TestGrains
{
    public class HelloTimers : Grain<HelloTimersState>, IGrainWithIntegerKey
    {
        private IDisposable _timer0;
        private IDisposable _timer1;

        public override Task OnActivateAsync()
        {
            _timer0 = RegisterTimer(_ => OnTimer0(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            _timer1 = RegisterTimer(_ => OnTimer1(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            _timer0.Dispose();
            _timer1.Dispose();

            return base.OnDeactivateAsync();
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
    }
}