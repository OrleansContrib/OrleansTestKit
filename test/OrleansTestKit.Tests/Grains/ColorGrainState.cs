using System.Diagnostics;
using TestInterfaces;

namespace TestGrains;

/// <summary>The persisted state of a grain that tracks a color.</summary>
[DebuggerStepThrough]
public sealed class ColorGrainState
{
    /// <summary>Gets or sets the color.</summary>
    public Color Color { get; set; }

    /// <summary>Gets or sets the unique ID.</summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the persisted state has been initialized. This is <c>true</c> when
    ///     <see cref="Id"/> is not equal to <see cref="Guid.Empty"/>.
    /// </summary>
    public bool Initialized => this.Id != Guid.Empty;
}
