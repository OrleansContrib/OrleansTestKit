using Orleans.Runtime;
using TestInterfaces;

namespace TestGrains;

/// <summary>
///     A grain that tracks a color. Specifically, this is an <see cref="IColorGrain"/> implementation that uses an
///     Orleans storage provider and the <see cref="IPersistentState{TState}"/> persistence pattern.
/// </summary>
public sealed class ColorGrain : Grain, IColorGrain
{
    /// <summary>The persisted state.</summary>
    private readonly IPersistentState<ColorGrainState> state;

    /// <summary>Initializes a new instance of the <see cref="ColorGrainWithRepository"/> class.</summary>
    /// <param name="state">The persisted state.</param>
    public ColorGrain(
        [PersistentState("State")]
        IPersistentState<ColorGrainState> state) =>
        this.state = state ?? throw new ArgumentNullException(nameof(state));

    /// <summary>Asynchronously gets the color.</summary>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result returns the color or
    ///     <see cref="Color.Unknown"/> if the color has not yet been set.
    /// </returns>
    public Task<Color> GetColor() =>
        Task.FromResult(this.state.State.Color);

    /// <summary>
    ///     Asynchronously resets the color to <see cref="Color.Unknown"/>. This has no effect if the color has not yet
    ///     been set or if the color has already been reset.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ResetColor() =>
        this.state.State.Initialized
            ? this.state.ClearStateAsync()
            : Task.CompletedTask;

    /// <summary>Asynchronously sets the color.</summary>
    /// <param name="color">The new color.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     If <paramref name="color"/> is <see cref="Color.Unknown"/> or an invalid value.
    /// </exception>
    public Task SetColor(Color color)
    {
        if (!Enum.IsDefined(typeof(Color), color))
        {
            throw new ArgumentException("The color is invalid.", nameof(color));
        }

        if (color == Color.Unknown)
        {
            throw new ArgumentException("The color must not be \"unknown\".", nameof(color));
        }

        if (this.state.State.Initialized)
        {
            if (color == this.state.State.Color)
            {
                return Task.CompletedTask;
            }
        }
        else
        {
            this.state.State.Id = this.GetPrimaryKey();
        }

        this.state.State.Color = color;
        return this.state.WriteStateAsync();
    }
}
