using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Algorithms;

namespace Archimedes.Geometry.Primitives
{
    public partial class Polygon2
    {

        #region Polygon - Point (Contains)

        /// <summary>
        /// Does this polygon contain the given point?
        /// </summary>
        /// <param name="p">The Point to check</param>
        /// <param name="tolerance">The Point to check</param>
        /// <returns>True if the Point is contained in the Polygon</returns>
        public bool Contains(Vector2 p, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {

            if (!BoundingCircle.Contains(p, tolerance))
            {
                return false;
            }

            int counter = 0;
            int i;
            Vector2 p2;
            int N = _vertices.Count;

            var p1 = _vertices[0];
            for (i = 1; i <= N; i++)
            {
                p2 = _vertices[i % N];
                if (p.Y > Math.Min(p1.Y, p2.Y))
                {
                    if (p.Y <= Math.Max(p1.Y, p2.Y))
                    {
                        if (p.X <= Math.Max(p1.X, p2.X))
                        {
                            if (p1.Y != p2.Y) // TODO Handle tolerance!!
                            {
                                double xinters = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || p.X <= xinters)
                                    counter++;
                            }
                        }
                    }
                }
                p1 = p2;
            }
            return (counter % 2 != 0);
        }

        #endregion

        #region Polygon contains Polygon

        /// <summary>
        /// Is the given Polygon fully contained in this one?
        /// </summary>
        /// <param name="polygon"> </param>
        /// <param name="tolerance"> </param>
        /// <returns></returns>
        public bool Contains(Polygon2 polygon, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (polygon.ToVertices().All(x => Contains(x, tolerance)))
            {
                // When all vertices are contained in a convex polygon
                // we already know that the given vertices are inside
                // this polygon thus we are done
                if (IsConvex()) return true;

                // Otherwise, this is a concave polygon
                // we need to ensure, that the vertices do not intersect any line
                var otherLines = polygon.ToLines();
                return !IntersectLinesWith(otherLines, tolerance);
            }

            return false;
        }

        /// <summary>
        /// Returns true if one of the given lines intersects with this polygons border lines.
        /// </summary>
        /// <param name="otherLines"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private bool IntersectLinesWith(LineSegment2[] otherLines, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var myLines = this.ToLines();

            foreach (var myLine in myLines)
            {
                foreach (var other in otherLines)
                {
                    if (myLine.InterceptLineWith(other, tolerance))
                        return true;
                }
            }
            return false;
        }

        #endregion


        public bool HasCollision(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            return Intersect(other).Any();
        }

        public IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (other is LineSegment2)
            {
                return this.InterceptLine(other as LineSegment2, tolerance);
            }
            else if (other is Circle2)
            {
                // inverse call
                return ((Circle2) other).Intersect(this);
            }
            else if (other is Polygon2)
            {
                return InterceptPolygon(other as Polygon2, tolerance);
            }
            else if (other is Rectangle2)
            {
                return InterceptPolygon((other as Rectangle2).ToPolygon2(), tolerance);
            }
            else if (other is Arc)
            {
                // inverse call (arc handles this case for us)
                return other.Intersect(this, tolerance); 
            }
            else if (other is IShape)
            {
                return InterceptPolygon(((IShape)other).ToPolygon2(), tolerance);
            }
            throw new NotSupportedException("Polygon intersection with " + other.GetType().Name + " is not supported yet.");
        }

        #region Polygon - Polygon

        /// <summary>
        /// Polygon - Polygon intersection
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private IEnumerable<Vector2> InterceptPolygon(Polygon2 otherPolygon, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            var thisLines = ToLines();
            var otherLines = otherPolygon.ToLines();

            foreach (var line in thisLines)
            {
                foreach (var other in otherLines)
                {
                    intersections.AddRange(line.Intersect(other));
                }
            }
            return intersections; 
        }

        #endregion

        #region Polygon - Line

        /// <summary>
        /// Polygon - Line intersection
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private IEnumerable<Vector2> InterceptLine(LineSegment2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            var thisLines = ToLines();
            foreach (var line in thisLines)
            {
                var intersection = other.Intersect(line);
                intersections.AddRange(intersection);
            }
            return intersections;
        }

        #endregion

        #region Bounding Box and Circle

        /// <summary>
        /// Gets the bounding circle of this polygon
        /// </summary>
        public Circle2 BoundingCircle
        {
            get
            {
                if (_boundingCircle == null || _boundCircleChanged)
                {
                    double dist;
                    var middlePoint = this.MiddlePoint;
                    double longestDist = 0;

                    foreach (var vertex in this._vertices)
                    {
                        dist = LineSegment2.CalcLenght(middlePoint, vertex);
                        if (longestDist < dist)
                            longestDist = dist;
                    }
                    _boundingCircle = new Circle2(middlePoint, (float)longestDist);
                }
                return _boundingCircle;
            }
        }

        /// <summary>
        /// Returns the Boundingbox which Aera is the smallest
        /// </summary>
        public Rectangle2 FindSmallestBoundingBox()
        {
            return FindBoundingBox(new PolygonSmallestBoundingBoxAlgorythm());
        }

        /// <summary>
        /// Returns the Boundingbox which one side (width) is the smallest possible
        /// </summary>
        public Rectangle2 FindSmallestWidthBoundingBox()
        {
            return FindBoundingBox(new PolygonSmallestWidthBoundingBoxAlgorythm());
        }

        /// <summary>
        /// Find the Boundingbox with the given Algorythm
        /// </summary>
        /// <param name="boxfindingAlgorythm">Concrete implementation of the bounding finder Algorythm to use</param>
        /// <returns></returns>
        public Rectangle2 FindBoundingBox(IPolygonBoundingBoxAlgorythm boxfindingAlgorythm)
        {
            Rectangle2 rect = null;
            var hull = !IsConvex() ? ToConvexPolygon() : this;

            var vertices = boxfindingAlgorythm.FindBounds(hull);

            if (vertices.Count() == 4)
            {
                rect = new Rectangle2(vertices);
            }

            if (rect == null || rect.Size.IsEmpty)
            {
                // Smallest boundingboxfinder failed - use simple boundingbox instead
                rect = this.BoundingBox.ToRectangle();
            }

            return rect;
        }

        #endregion

    }
}
