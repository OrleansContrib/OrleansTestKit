using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using TestInterfaces;

namespace TestGrains
{
    public sealed class LegacyLoggerGrain : Grain, ILegacyLoggerGrain
    {
        private Logger customLogger;
        private Logger defaultLogger;

        public override Task OnActivateAsync()
        {
            this.defaultLogger = this.GetLogger();
            this.customLogger = this.GetLogger("Custom");

            return base.OnActivateAsync();
        }

        public Task WriteToCustomLog(string message)
        {
            this.customLogger.Info(message);
            return Task.CompletedTask;
        }

        public Task WriteToDefaultLog(string message)
        {
            this.defaultLogger.Info(message);
            return Task.CompletedTask;
        }
    }
}
