using System.Numerics;

namespace Fletch.Physics.Model.ShapeData
{
    internal class CapsuleData : ShapeData
    {
        public float Radius { get; }

        public float Height { get; }

        public CapsuleData(float radius, float height)
        {
            Radius = radius;
            Height = height;
        }
    }
}
