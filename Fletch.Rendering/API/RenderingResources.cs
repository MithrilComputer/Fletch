using Fletch.Rendering.Abstractions.Backends;
using Fletch.Rendering.Abstractions.Drawing;
using Fletch.Rendering.Abstractions.Factories;

namespace Fletch.Rendering.API
{
    public class RenderingResources
    {
        public IDebugRenderer DebugRenderer { get; }
        
        public ITextureFactory TextureFactory { get; }

        public IFontFactory FontFactory { get; }

        internal RenderingResources(IRenderingBackend backend)
        {
            DebugRenderer = backend.DebugRenderer;
            TextureFactory = backend.TextureFactory;
            FontFactory = backend.FontFactory;
        }
    }
}
