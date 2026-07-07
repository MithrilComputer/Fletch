namespace Fletch.Physics.Model.Info.Shapes
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
