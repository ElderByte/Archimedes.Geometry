using System;
using System.Collections.Generic;
using System.Linq;

namespace Archimedes.Geometry.Primitives
{
    public partial class LineSegment2 : IGeometry
    {
        #region Line - Line Intersection

        /// <summary>
        /// Calculates the interception point of two Lines.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns>Returns a Point if func succeeds. If there is no interception, empty point is returned.</returns>
        private Vector2? IntersectLine(LineSegment2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {

            double interPntX = 0;
            double interPntY = 0;

            if (other == null || this.IsParallelTo(other, tolerance))
            {
                return null;            // Lines are parralell
            }

            //intercept of two endless lines
            if (!this.IsVertical && !other.IsVertical) {    // both NOT vertical
                interPntX = ((-1 * (this.YMovement - other.YMovement)) / (this.Slope - other.Slope));
                interPntY = (this.Slope * interPntX + this.YMovement);
            } else if (this.IsVertical) {                  // this vertical (so it must lie on this.X)
                interPntX = this.Start.X;
                interPntY = (other.Slope * interPntX + other.YMovement);
            } else if (other.IsVertical) {                // Line2 vertical (so it must lie on Line2.X)
                interPntX = other.Start.X;
                interPntY = (this.Slope * interPntX + this.YMovement);
            }

            var interPnt = new Vector2(interPntX, interPntY);

            //check if computed intercept lies on our line.
            if (this.Contains(interPnt, tolerance) && other.Contains(interPnt, tolerance))
            {
                return interPnt;
            }else
                return null;
        }

        /// <summary>
        /// Is there a intersection?
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns>Returns true/false.</returns>
        public bool HasIntersection(LineSegment2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            return IntersectLine(other, tolerance).HasValue;
        }

        /// <summary>
        /// Checks if this line segment properly intresects another line semgent.
        /// The intersection between two line segments is considered proper if they intersect in a single point in the interior of both segments (e.g. the intersection is a single point and is not equal to any of the endpoints).
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool IsIntersectionProper(LineSegment2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            // If the lines are parallel, there cant be a proper intersection
            if (this.IsParallelTo(other, tolerance)) return false;

            var intersecitonPoints = Intersect(other, tolerance).ToList();
            if (intersecitonPoints.Count == 1)
            { // There must only be one intersection point

                var intersectionPoint = intersecitonPoints.First();
                // This intersection point must now lie on the interior of both lines, i.e. not on any endpoint of both lines.

                if (this.Start.Equals(intersectionPoint, tolerance) || this.End.Equals(intersectionPoint, tolerance)
                    || other.Start.Equals(intersectionPoint, tolerance) || other.End.Equals(intersectionPoint, tolerance))
                {
                    return false; // The intersection is on the line start/end point - thus they only meet.
                }
                return true;
            }
            return false;
        }

        #endregion

        // Can be removed if Polygon2 supports Line intersections
        #region Line - AARectangle Intersection

        /// <summary>
        /// Returns true if there is at least one intersection point.
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool InterceptRectWith(AARectangle rect1, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            
            // is Start/EndPoint in the Rectangle?
            if (rect1.Contains(this.Start) || rect1.Contains(this.End)) // TODO handle collision
                return true; 
            // crosses the Line a Rectangle Border?
            var borderLines = FromRectangle(rect1); //get 4 borderlines from rect

            // check if any of the borderlines intercept with this line
            return borderLines.Any(border => this.HasIntersection(border, tolerance));
        }


        /// <summary>
        /// Returns every intersection Point of the Rect::Line if any. Max returned Points are 2.
        /// This Method actaully can't handle full contained Lines in the Rect - use the InterceptRectWith to determite if there is collision.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="tolerance"></param>
        /// <returns>Returns count of Interception Points</returns>
        public List<Vector2> InterceptRect(AARectangle rect, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intercepts = new List<Vector2>(2);
            short i = 0;
            var borderLines = FromRectangle(rect); //get 4 borderlines from rect

            foreach (var border in borderLines) {
                if (this.HasIntersection(border, tolerance))
                {
                    // found interception
                    var pnt = this.IntersectLine(border, tolerance);
                    if(pnt.HasValue)
                        intercepts.Add(pnt.Value);
                    i++;
                    if (i == 2) break;  
                }
            }
            return intercepts;
        }

        #endregion

        #region Line - Point (Contains)

        /// <summary>
        /// Checks if a Point is on the 2dLine (or in its range)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="tolerance"></param>
        /// <returns>true/false</returns>
        public bool Contains(Vector2 pos, double tolerance = GeometrySettings.DEFAULT_TOLERANCE) {

            // We can't check directly with math functions,
            // as it is possible that the slope is undefinied. (on vertical lines)

            if (this.IsVertical) {
                // Vertical means that the slope is undefinied
                if (Math.Abs(this.Start.X - pos.X) <= tolerance)
                {
                    return (((pos.Y >= this.Start.Y) && (pos.Y <= this.End.Y)) || ((pos.Y <= this.Start.Y) && (pos.Y >= this.End.Y)));
                } else 
                    return false;
            } else {
                if (Math.Abs(pos.Y - (pos.X * this.Slope + this.YMovement)) <= tolerance)
                {
                    return (((pos.X >= this.Start.X) && (pos.X <= this.End.X)) || ((pos.X <= this.Start.X) && (pos.X >= this.End.X)));
                } else
                    return false;
            }
        }

        #endregion
    }
}
