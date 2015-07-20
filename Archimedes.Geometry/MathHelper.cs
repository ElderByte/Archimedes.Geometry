
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry
{
    public static class MathHelper
    {

        /*
        public static Matrix FlipY {
            get {
                return new Matrix(1, 0, 0,
                                 -1, 0, 0);
            }
        }
        public static Matrix FlipX {
            get {
                return new Matrix(-1, 0, 0,
                                   1, 0, 0);
            }
        }
        */

        public static void GetStartEndPoint(IGeometry geometry, out Vector2? start, out Vector2? end)
        {

            start = null;
            end = null;

            if (geometry is LineSegment2) {
                var line = geometry as LineSegment2;
                start = line.Start; ;
                end = line.End;
            } else if (geometry is Arc) {
                var arc = geometry as Arc;
                start = arc.Location;
                end = arc.EndPoint;
            }
        }



    }
}
