using System;
using System.Threading.Tasks;
using Orleans;
using TestInterfaces;

namespace TestGrains
{
    public sealed class DIGrain : Grain, IGrainWithGuidKey
    {
        public DIGrain(IDIService service)
        {
            Service = service;
        }

        public IDIService Service { get; }

        public bool GetServiceValue() => Service.GetValue();
    }

    public interface IDIService
    {
        bool GetValue();
    }
}
