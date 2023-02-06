using Orleans.Core;
using Orleans.TestKit.Storage;

namespace Orleans.TestKit;

public sealed class TestKitOptions
{
    /// <summary>
    ///     Gets or sets a custom <see cref="IStorage{TState}"/> object factory. The default
    ///     <see cref="TestStorage{TState}"/> is used if <see langword="null"/>. The custom
    ///     <see cref="IStorage{TState}"/> object may implement <see cref="IStorageStats"/> to capture and report
    ///     statistics.
    /// </summary>
    public Func<Type, object>? StorageFactory { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether strict Moq probes are enabled for grains. When
    ///     <see langword="true"/>, all probes must be explicitly added to be used.
    /// </summary>
    public bool StrictGrainProbes { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether strict Moq probes are enabled for services. When
    ///     <see langword="true"/>, all probes must be explicitly added to be used.
    /// </summary>
    public bool StrictServiceProbes { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether strict Moq probes are enabled for streams. When
    ///     <see langword="true"/>, all probes must be explicitly added to be used.
    /// </summary>
    public bool StrictStreamProbes { get; set; }
}
