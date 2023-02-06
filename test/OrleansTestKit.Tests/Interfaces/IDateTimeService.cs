namespace TestInterfaces;

public interface IDateTimeService
{
    Task<DateTime> GetCurrentDate();
}
