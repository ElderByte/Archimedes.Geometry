using System.Collections.Generic;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry
{
    public static class VerticesHelper
    {
        /// <summary>
        /// Builds Line-Paths from vertices - <example>3 <paramref name="vertices"/> give 2 Lines</example>
        /// </summary>
        /// <param name="vertices">Sorted vertices</param>
        /// <returns>Line(s)</returns>
        public static IEnumerable<LineSegment2> VerticesPathToLines(IEnumerable<Vector2> vertices) {
            var segments = new List<LineSegment2>();
            Vector2? startPoint = null;
            Vector2? endPoint = null;
            int i = 0;
            foreach (var uP in vertices) {
                i++;
                if (i == 1) {
                    startPoint = uP;
                    continue;
                } else
                    endPoint = uP;

                if (startPoint.HasValue && endPoint.HasValue) {
                    segments.Add(new LineSegment2(startPoint.Value, endPoint.Value));
                }
                startPoint = endPoint;
            }
            return segments;
        }

        /// <summary>
        /// Sorts the vertices X and y oriented
        /// </summary>
        /// <param name="vertices"></param>
        public static void SortVertices(List<Vector2> vertices) {
            vertices.Sort(delegate(Vector2 p1, Vector2 p2)
            {
                int dx = p1.X.CompareTo(p2.X);
                if (dx != 0) {
                    return dx;
                } else {
                    return p1.Y.CompareTo(p2.Y);
                }
            });
        }



    }
}
