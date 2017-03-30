using System;
using System.Threading.Tasks;

namespace TestInterfaces
{
    public interface IDateTimeService
    {
        Task<DateTime> GetCurrentDate();
    }
}