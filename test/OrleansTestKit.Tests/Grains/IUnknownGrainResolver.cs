namespace TestGrains;

public interface IUnknownGrainResolver : IGrainWithStringKey
{
    Task CreateAndPingMultiple();

    Task<List<string>> GetResolvedUnknownGrainIdsAsync();
}
