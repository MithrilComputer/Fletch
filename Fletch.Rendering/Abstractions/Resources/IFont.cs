using System.Numerics;

namespace Fletch.Rendering.Abstractions.Resources
{
    /// <summary>
    /// Represents a font resource usable by the rendering system.
    /// </summary>
    internal interface IFont
    {
        /// <summary>
        /// Measures the size of the given text in pixels.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>The width and height in pixels.</returns>
        Vector2 MeasureString(string text);

        /// <summary>
        /// The font line spacing (distance between baselines) in pixels.
        /// </summary>
        float LineSpacing { get; }

        /// <summary>
        /// The font name or identifier (optional, for debugging).
        /// </summary>
        string Name { get; }
    }
}
