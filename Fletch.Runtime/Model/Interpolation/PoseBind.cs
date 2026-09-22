using Fletch.Physics.Components;
using Fletch.Rendering.Components;

namespace Fletch.Runtime.Model.Interpolation
{
    internal class PoseBind
    {
        public bool IsValid => RigidBody != null && RenderingComponents.Count > 0;

        public RigidBody? RigidBody { get; set; }

        public HashSet<IInternalRenderingComponent> RenderingComponents { get; set; } = new HashSet<IInternalRenderingComponent>();
    }
}
