using System.Numerics;

namespace Fletch.Physics.Model.ShapeData
{
    internal class CircleData : ShapeData
    {
        public float Radius { get; }

        public CircleData(float radius)
        {
            Radius = radius;
        }
    }
}
