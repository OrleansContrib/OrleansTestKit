using TestInterfaces;

namespace TestGrains;

public class HelloGrainWithServiceDependency : Grain, IHello
{
    private readonly IDateTimeService _dateTimeService;

    public HelloGrainWithServiceDependency(IDateTimeService dateTimeService)
    {
        if (dateTimeService == null)
            throw new ArgumentNullException(nameof(dateTimeService));

        _dateTimeService = dateTimeService;
    }

    public async Task<string> SayHello(string greeting)
    {
        var date = await _dateTimeService.GetCurrentDate();

        return $"[{date.Date}]: You said: '" + greeting + "', I say: Hello!";
    }
}
