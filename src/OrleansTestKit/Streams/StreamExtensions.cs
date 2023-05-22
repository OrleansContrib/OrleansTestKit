using Orleans.Runtime;
using Orleans.TestKit.Streams;

namespace Orleans.TestKit;

/// <summary>
/// Extensions for adding stream probes to the TestKitSilo
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo) =>
        AddStreamProbe<T>(silo, Guid.Empty);

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id) =>
        AddStreamProbe<T>(silo, id, typeof(T).Name);

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <param name="streamNamespace">The stream namespace.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string? streamNamespace) =>
        AddStreamProbe<T>(silo, id, streamNamespace, "Default");

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <param name="streamNamespace">The stream namespace.</param>
    /// <param name="providerName">The stream provider</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, Guid id, string? streamNamespace, string providerName) =>
        AddStreamProbe<T>(silo, StreamId.Create(streamNamespace ?? string.Empty, id), providerName);

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, string id) =>
        AddStreamProbe<T>(silo, id, typeof(T).Name);

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <param name="streamNamespace">The stream namespace.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, string id, string? streamNamespace) =>
        AddStreamProbe<T>(silo, id, streamNamespace, "Default");

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <param name="streamNamespace">The stream namespace.</param>
    /// <param name="providerName">The stream provider</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, string id, string? streamNamespace, string providerName) =>
        AddStreamProbe<T>(silo, StreamId.Create(streamNamespace ?? string.Empty, id), providerName);

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, StreamId id) =>
        AddStreamProbe<T>(silo, id, "Default");

    /// <summary>
    /// Adds a stream probe for Guid.Empty
    /// </summary>
    /// <typeparam name="T">The stream type.</typeparam>
    /// <param name="silo">The test kit silo.</param>
    /// <param name="id">The stream id.</param>
    /// <param name="providerName">The stream provider</param>
    /// <returns>The stream probe</returns>
    public static TestStream<T> AddStreamProbe<T>(this TestKitSilo silo, StreamId id, string providerName)
    {
        if (silo == null)
        {
            throw new ArgumentNullException(nameof(silo));
        }

        if (providerName == null)
        {
            throw new ArgumentNullException(nameof(providerName));
        }

        return silo.StreamProviderManager.AddStreamProbe<T>(id, providerName);
    }
}
