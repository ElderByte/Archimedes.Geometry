using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Primitives
{
    public partial class Circle2
    {
        #region Circle - Point (Contains)

        public virtual bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            return Circle2.Contains(this.MiddlePoint, this.Radius, point, tolerance);
        }

        /// <summary>
        /// Checks if a Point lies in a circle
        /// (This is a strictly static method for performance reasons)
        /// </summary>
        /// <param name="middlePoint"></param>
        /// <param name="radius"></param>
        /// <param name="checkPoint"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool Contains(Vector2 middlePoint, double radius, Vector2 checkPoint, double tolerance)
        {
            var len = LineSegment2.CalcLenght(middlePoint, checkPoint);
            var dist = radius - (len - tolerance);
            return (dist >= 0);
        }

        #endregion

        #region Cricle - Line

        /// <summary>Circle-Line Interception
        /// Does the Line intercept with the Circle?
        /// </summary>
        /// <param name="uLine"></param>
        /// <returns></returns>
        private bool InterceptLineWith(LineSegment2 uLine, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = InterceptLine(uLine, tolerance);
            return intersections.Any();
        }

        /// <summary> 
        /// Circle-Line Interception 
        /// </summary>
        /// <param name="uLine"></param>
        /// <returns>Returns all intersection points</returns>
        private List<Vector2> InterceptLine(LineSegment2 uLine, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            Vector2 p1, p2;
            var location = this.Location;

            // we assume that the circle Middlepoint is NULL/NULL
            // So we move the Line with the delta to NULL
            var helperLine = new LineSegment2(uLine.Start - location, uLine.End - location);

            // line
            var q = helperLine.YMovement;
            var m = helperLine.Slope;

            if (!helperLine.IsVertical)
            {
                // The slope is defined as the Line isn't vertical

                var discriminant = (Math.Pow(m, 2) + 1) * Math.Pow(this.Radius, 2) - Math.Pow(q, 2);
                if (discriminant > 0)
                {
                    // only positive discriminants for f() -> sqrt(discriminant) results are defined in |R



                    var p1X = (Math.Sqrt(discriminant) - m * (q)) / (Math.Pow(m, 2) + 1);
                    var p1Y = m * p1X + q;
                    var p2X = (-1 * (Math.Sqrt(discriminant) + m * q)) / (Math.Pow(m, 2) + 1);
                    var p2Y = m * p2X + q;

                    p1 = new Vector2(p1X, p1Y);
                    p2 = new Vector2(p2X, p2Y);

                    if (helperLine.Contains(p1, tolerance))
                    {
                        intersections.Add(p1 + location);
                    }
                    if ((p1.X != p2.X) || (p1.Y != p2.Y))
                    {
                        if (helperLine.Contains(p2, tolerance))
                        {
                            intersections.Add(p2 + location);
                        }
                    }
                }
            }
            else
            {
                // undefined slope, so we have to deal with it directly

                var p1X = this.Location.X + helperLine.Start.X;
                var p1Y = Math.Sqrt(Math.Pow(this.Radius, 2) - Math.Pow(p1X, 2));
                p1 = new Vector2(p1X, p1Y);
                p2 = new Vector2(p1.X, -p1.Y);

                if (helperLine.Contains(p1, tolerance))
                {
                    intersections.Add(p1 + location);
                }
                if (helperLine.Contains(p2, tolerance))
                {
                    intersections.Add(p2 + location);
                }
            }

            return intersections;
        }

        #endregion

        #region Circle - Polygon

        /// <summary>
        /// Finds all intersection points with the given polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private List<Vector2> InterceptPolygon(Polygon2 polygon, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            foreach (var border in polygon.ToLines())
            {
                intersections.AddRange(this.InterceptLine(border, tolerance));
            }
            return intersections;
        }

        #endregion

        #region Circle - Circle

        private bool InterceptWithCircle(Circle2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var middlepointDistance = LineSegment2.CalcLenght(this.MiddlePoint, other.MiddlePoint);
            var radiusSum = this.Radius + other.Radius;
            return !(middlepointDistance > (radiusSum + tolerance));
        }

        private IEnumerable<Vector2> InterceptCircle(Circle2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var interceptions = new List<Vector2>();
            if (InterceptWithCircle(other, tolerance))
            {
                var middlepointDistance = LineSegment2.CalcLenght(this.MiddlePoint, other.MiddlePoint);
                if (middlepointDistance < Math.Abs(this.Radius + other.Radius))
                {
                    // circle is contained in other
                }
                else if (middlepointDistance == 0 && (this.Radius == other.Radius))
                {
                    // circle are concident -> infinite numbers of intersections
                }
                else
                {
                    interceptions.AddRange(IntersectCircle(this, other));
                }
            }
            return interceptions;
        }

        /// <summary>
        /// Returns the radius^2
        /// </summary>
        private double Radius2
        {
            get { return Math.Pow(this.Radius, 2); }
        }

        /// <summary>
        /// Find Circle - Circle Intersectionpoints
        /// </summary>
        /// <param name="cA"></param>
        /// <param name="cB"></param>
        /// <returns></returns>
        private IEnumerable<Vector2> IntersectCircle(Circle2 cA, Circle2 cB)
        {

            var dv = cA.MiddlePoint - cB.MiddlePoint;
            var d2 = dv.X * dv.X + dv.Y * dv.Y;
            var d = Math.Sqrt(d2);

            if (d > cA.Radius + cB.Radius || d < Math.Abs(cA.Radius - cB.Radius))
                return new Vector2[] { }; // no solution

            var a = (cA.Radius2 - cB.Radius2 + d2) / (2 * d);
            var h = Math.Sqrt(cA.Radius2 - a * a);
            var x2 = cA.MiddlePoint.X + a * (cB.MiddlePoint.X - cA.MiddlePoint.X) / d;
            var y2 = cA.MiddlePoint.Y + a * (cB.MiddlePoint.Y - cA.MiddlePoint.Y) / d;

            var paX = x2 + h * (cB.MiddlePoint.Y - cA.MiddlePoint.Y) / d;
            var paY = y2 - h * (cB.MiddlePoint.X - cA.MiddlePoint.X) / d;
            var pbX = x2 - h * (cB.MiddlePoint.Y - cA.MiddlePoint.Y) / d;
            var pbY = y2 + h * (cB.MiddlePoint.X - cA.MiddlePoint.X) / d;

            return new Vector2[] { new Vector2(paX, paY), new Vector2(pbX, pbY) };
        }

        #endregion

        #region Bounding box and circle

        public virtual Circle2 BoundingCircle
        {
            // bounding circle of a cricle... really abstract :)
            get { return this.Clone() as Circle2; }
        }

        public virtual AARectangle BoundingBox
        {
            get
            {
                return new AARectangle(
                    (this.Location.X - this.Radius), (this.Location.Y - this.Radius),
                    (2.0 * this.Radius), (2.0 * this.Radius));
            }
        }

        #endregion
    }
}
