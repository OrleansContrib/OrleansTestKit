using Orleans.Runtime;

namespace Orleans.TestKit;
/// <summary>
/// Extension methods for creating grains from strongly typed IDs
/// </summary>
public static class TestKitSiloGrainCreationExtensions
{
    /// <summary>
    /// Create a grain - will be injected with a grain context and participate in grain lifecycle
    /// </summary>
    /// <typeparam name="T">The grain type (class not interface)</typeparam>
    /// <param name="silo">The test kit silo</param>
    /// <param name="id">The grain's long id</param>
    /// <returns>The grain</returns>
    public static Task<T> CreateGrainAsync<T>(this TestKitSilo silo, long id)
        where T : Grain, IGrainWithIntegerKey =>
        silo.CreateGrainAsync<T>(GrainIdKeyExtensions.CreateIntegerKey(id));

    /// <summary>
    /// Create a grain - will be injected with a grain context and participate in grain lifecycle
    /// </summary>
    /// <typeparam name="T">The grain type (class not interface)</typeparam>
    /// <param name="silo">The test kit silo</param>
    /// <param name="id">The grain's id</param>
    /// <returns>The grain</returns>
    public static Task<T> CreateGrainAsync<T>(this TestKitSilo silo, Guid id)
        where T : Grain, IGrainWithGuidKey =>
        silo.CreateGrainAsync<T>(GrainIdKeyExtensions.CreateGuidKey(id));

    /// <summary>
    /// Create a grain - will be injected with a grain context and participate in grain lifecycle
    /// </summary>
    /// <typeparam name="T">The grain type (class not interface)</typeparam>
    /// <param name="silo">The test kit silo</param>
    /// <param name="id">The grain's id</param>
    /// <returns>The grain</returns>
    public static Task<T> CreateGrainAsync<T>(this TestKitSilo silo, string id)
        where T : Grain, IGrainWithStringKey
        => silo.CreateGrainAsync<T>(IdSpan.Create(id));

    /// <summary>
    /// Create a grain - will be injected with a grain context and participate in grain lifecycle
    /// </summary>
    /// <typeparam name="T">The grain type (class not interface)</typeparam>
    /// <param name="silo">The test kit silo</param>
    /// <param name="id">The grain's id</param>
    /// <param name="keyExtension">The key extension</param>
    /// <returns>The grain</returns>
    public static Task<T> CreateGrainAsync<T>(this TestKitSilo silo, Guid id, string keyExtension)
        where T : Grain, IGrainWithGuidCompoundKey
        => silo.CreateGrainAsync<T>(GrainIdKeyExtensions.CreateGuidKey(id, keyExtension));

    /// <summary>
    /// Create a grain - will be injected with a grain context and participate in grain lifecycle
    /// </summary>
    /// <typeparam name="T">The grain type (class not interface)</typeparam>
    /// <param name="silo">The test kit silo</param>
    /// <param name="id">The grain's id</param>
    /// <param name="keyExtension">The key extension</param>
    /// <returns>The grain</returns>
    public static Task<T> CreateGrainAsync<T>(this TestKitSilo silo, long id, string keyExtension)
        where T : Grain, IGrainWithIntegerCompoundKey
        => silo.CreateGrainAsync<T>(GrainIdKeyExtensions.CreateIntegerKey(id, keyExtension));
}
