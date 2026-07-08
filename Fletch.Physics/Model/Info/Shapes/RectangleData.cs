using System.Numerics;

namespace Fletch.Physics.Model.Info.Shapes
{
    internal class RectangleData : ShapeData
    {
        public Vector2 Size { get; }

        public RectangleData(Vector2 size)
        {
            Size = size;
        }
    }
}
