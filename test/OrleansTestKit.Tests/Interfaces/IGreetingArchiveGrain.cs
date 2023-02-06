namespace TestInterfaces;

/// <summary>Represents an Orleans grain that stores a list of greetings.</summary>
public interface IGreetingArchiveGrain : IGrainWithIntegerKey
{
    Task AddGreeting(string greeting);

    Task<IEnumerable<string>> GetGreetings();

    Task ResetGreetings();
}
