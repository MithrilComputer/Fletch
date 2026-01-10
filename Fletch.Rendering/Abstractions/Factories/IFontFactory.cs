using Fletch.Rendering.Abstractions.Resources;

namespace Fletch.Rendering.Abstractions.Factories
{
    internal interface IFontFactory
    {
        /// <summary>
        /// Registers a font family from a TrueType Font (.ttf) file.
        /// </summary>
        /// <param name="name">Logical font family name used for lookup.</param>
        /// <param name="ttfPath">File system path to the .ttf font file.</param>
        /// <remarks>
        /// If a font family with the same name already exists, it will be replaced.
        /// </remarks>
        void RegisterFamily(string name, string ttfPath);

        /// <summary>
        /// Gets a font from the specified family and size.
        /// Creates and caches the font if it has not already been created.
        /// </summary>
        /// <param name="family">Registered font family name.</param>
        /// <param name="size">Point size of the requested font.</param>
        /// <returns>An <see cref="IFont"/> matching the specified family and size.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when the requested font family has not been registered.
        /// </exception>
        IFont GetFont(string family, int size);

        /// <summary>
        /// Attempts to get a font from the specified family and size without throwing.
        /// </summary>
        /// <param name="family">Registered font family name.</param>
        /// <param name="size">Point size of the requested font.</param>
        /// <param name="font">
        /// When this method returns, contains the font if it was found or created;
        /// otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the font could be returned; otherwise <c>false</c>.
        /// </returns>
        bool TryGetFont(string family, int size, out IFont? font);
    }
}
