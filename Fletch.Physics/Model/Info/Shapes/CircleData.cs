namespace Fletch.Physics.Model.Info.Shapes
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
