using System.Numerics;

namespace Fletch.Physics.Model.ShapeData
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
