namespace TestInterfaces;

/// <summary>Represents a grain that tracks a color.</summary>
public interface IColorGrain : IGrainWithGuidKey
{
    /// <summary>Asynchronously gets the color.</summary>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result returns the color or
    ///     <see cref="Color.Unknown"/> if the color has not yet been set.
    /// </returns>
    Task<Color> GetColor();

    /// <summary>
    ///     Asynchronously resets the color to <see cref="Color.Unknown"/>. This has no effect if the color has not yet
    ///     been set or if the color has already been reset.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ResetColor();

    /// <summary>Asynchronously sets the color.</summary>
    /// <param name="color">The new color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="color"/> is <see cref="Color.Unknown"/> or an invalid value.
    /// </exception>
    Task SetColor(Color color);
}
