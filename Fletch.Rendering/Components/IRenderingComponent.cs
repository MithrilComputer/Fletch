using Fletch.Rendering.Model;

namespace Fletch.Rendering.Components
{
    public interface IRenderingComponent
    {
        RenderPose RenderPose { get; }
    }

    internal interface IInternalRenderingComponent : IRenderingComponent
    {
        void SetRenderPose(RenderPose pose);
    }
}
