using System.Numerics;

namespace Fletch.Core.Math.Geometry
{
    /// <summary>
    /// Represents a 2D rotation used by transforms.
    /// Stores both degrees and radians, kept in sync.
    /// </summary>
    public class Rotation
    {
        private const float radToDeg = 180f / MathF.PI;

        private const float degToRad = MathF.PI / 180f;

        /// <summary>
        /// Current rotation angle in radians (-π to +π).
        /// </summary>
        public float Radians { get; private set; }

        /// <summary>
        /// Current rotation angle in degrees (-180 to +180).
        /// </summary>
        public float Degrees { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Rotation"/> with the specified angle in Degrees.
        /// </summary>
        /// <param name="degrees">Init angle in Degrees</param>
        public Rotation(float degrees)
        {
            SetDegrees(degrees);
        }

        /// <summary>
        /// Creates a new <see cref="Rotation"/> at 0 degrees.
        /// </summary>
        public Rotation()
        {
            SetDegrees(0f);
        }

        /// <summary>
        /// Sets rotation to face from a start point toward a target point.
        /// </summary>
        public void PointAt(Vector2 start, Vector2 target)
        {
            Vector2 d = target - start;
            SetRadians(MathF.Atan2(d.Y, d.X));
        }

        /// <summary>
        /// Rotates by a number of degrees.
        /// </summary>
        public void RotateDegrees(float angle)
        {
            SetDegrees(Degrees + angle);
        }

        /// <summary>
        /// Rotates by a number of radians.
        /// </summary>
        public void RotateRadian(float angle)
        {
            SetRadians(Radians + angle);
        }

        /// <summary>
        /// Sets the rotation using degrees.
        /// </summary>
        public void SetDegrees(float angle)
        {
            float wrappedDegrees = WrapDegreesMinus180ToPlus180(angle);

            Degrees = wrappedDegrees;
            Radians = wrappedDegrees * degToRad;
        }

        /// <summary>
        /// Sets the rotation using radians.
        /// </summary>
        public void SetRadians(float angle)
        {
            float wrappedRadians = WrapRadiansMinusPiToPlusPi(angle);

            Radians = wrappedRadians;
            Degrees = wrappedRadians * radToDeg;
        }

        /// <summary>
        /// Converts the rotation into a unit direction vector.
        /// </summary>
        public Vector2 ToVector()
        {
            return new Vector2(MathF.Cos(Radians), MathF.Sin(Radians));
        }

        /// <summary>
        /// Sets rotation from a direction vector.
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (direction.LengthSquared() <= 1e-12f) return;
            SetRadians(MathF.Atan2(direction.Y, direction.X));
        }

        /// <summary>
        /// Returns the shortest signed angle difference in degrees.
        /// </summary>
        public static float DeltaDegrees(float fromDegrees, float toDegrees)
        {
            return WrapDegreesMinus180ToPlus180(toDegrees - fromDegrees);
        }

        /// <summary>
        /// Wraps degrees into the range -180 to +180.
        /// </summary>
        private static float WrapDegreesMinus180ToPlus180(float degrees)
        {
            float wrappedDegrees = degrees % 360f;

            if (wrappedDegrees > 180f)
            {
                wrappedDegrees -= 360f;
            }
            else if (wrappedDegrees < -180f)
            {
                wrappedDegrees += 360f;
            }

            return wrappedDegrees;
        }

        /// <summary>
        /// Wraps radians into the range -pi to +pi.
        /// </summary>
        private static float WrapRadiansMinusPiToPlusPi(float radians)
        {
            const float twoPi = MathF.PI * 2f;
            float wrappedRadians = radians % twoPi;

            if (wrappedRadians > MathF.PI)
            {
                wrappedRadians -= twoPi;
            }
            else if (wrappedRadians < -MathF.PI)
            {
                wrappedRadians += twoPi;
            }

            return wrappedRadians;
        }
    }
}
