using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry
{
    public static class VectorHelper
    {
        public static Vector2 GetEndVector(IGeometry g) {
            Vector2 v;
            if (g is LineSegment2) {
                v = (g as LineSegment2).ToVector();
            } else if (g is Arc) {
                v = (g as Arc).EndVector;
            } else
                throw new NotSupportedException("Can't get End-Vector from IGeometryBase: " + g.GetType().ToString());
            return v;
        }

        /// <summary>
        /// Returns a signed angle
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public static Angle GetSignedAngleBetween(LineSegment2 direction, Vector2 next)
        {
            var angle = next.GetAngleTo(direction.ToVector());
            return angle * (direction.IsLeft(next) ? 1 : -1);
        }

    }
}
