using System;
using System.Collections.Generic;
using System.Linq;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// Collision parts of Arc implementation
    /// </summary>
    public partial class Arc
    {

        #region Rectangle2


        /// <summary>
        /// Checks if a Rectangle collides with the Arc.
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>On collision, true is returned, false otherwise</returns>
        private bool InterceptWithPolygon(Polygon2 polygon, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var borderLines = polygon.ToLines(); //get 4 borderlines from rect
            if (borderLines != null) {
                return InterceptLinesWith(borderLines, tolerance);
            }
            return false;
        }

        /// <summary>Checks if a Rectangle collides with the Arc.
        /// 
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>On collision, a List of interception Points is returned</returns>
        private List<Vector2> InterceptPolygon(Polygon2 polygon, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            var borderLines = polygon.ToLines(); //get 4 borderlines from rect
            if (borderLines != null) {
                intersections.AddRange(InterceptLines(borderLines, tolerance));
            }
            return intersections;
        }

        #endregion

        #region Arc - AARectangle

        /// <summary>
        /// Checks if a Rectangle collides with the Arc.
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>On collision, true is returned, false otherwise</returns>
        private bool InterceptRectWith(AARectangle rect, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            
            var borderLines = LineSegment2.FromRectangle(rect); //get 4 borderlines from rect
            if (borderLines != null) {
                return InterceptLinesWith(borderLines, tolerance);
            }
            return false;
        }

        /// <summary>Checks if a Rectangle collides with the Arc.
        /// 
        /// </summary>
        /// <param name="rect">Rectangle to check</param>
        /// <returns>On collision, a List of interception Points is returned</returns>
        private List<Vector2> InterceptRect(AARectangle rect, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            var borderLines = LineSegment2.FromRectangle(rect); //get 4 borderlines from rect
            if (borderLines != null) {
                intersections.AddRange(InterceptLines(borderLines, tolerance));
            }
            return intersections;
        }

        #endregion

        #region Arc - Line

        /// <summary>
        /// Arc-Line Interception
        /// </summary>
        /// <param name="uLine">Line to check</param>
        /// <param name="tolerance"></param>
        /// <returns>Returns the interception Point(s) if the Objects collide</returns>
        private List<Vector2> InterceptLine(LineSegment2 uLine, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();

            // Stretch the line on both ends by tolerance
            // TODO Is this still necessary??
            var strechtedLine = (uLine.Clone() as LineSegment2);
            strechtedLine.Stretch(tolerance, Direction.LEFT);
            strechtedLine.Stretch(tolerance, Direction.RIGHT);
            uLine = strechtedLine;


            // Intersect with a circle and test inter points if they lie on our arc
            var circle = new Circle2(this.MiddlePoint, this.Radius);
            foreach (var possiblePnt in circle.Intersect(uLine, tolerance))
            {
                if (this.Contains(possiblePnt, tolerance))
                {
                    intersections.Add(possiblePnt);
                }
            }
            
            return intersections;
        }

        /// <summary>
        /// Arc - Line Collision
        /// </summary>
        /// <param name="uLine">Line to check</param>
        /// <returns>Returns true, if the objects collide, false otherwise</returns>
        private bool InterceptLineWith(LineSegment2 uLine, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            return InterceptLine(uLine, tolerance).Any();
        }


        private List<Vector2> InterceptLines(IEnumerable<LineSegment2> lines, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            foreach (var border in lines)
            {
                intersections.AddRange(this.InterceptLine(border, tolerance));
            }
            return intersections;
        }

        private bool InterceptLinesWith(IEnumerable<LineSegment2> lines, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            foreach (var border in lines)
            {
                if (this.InterceptLine(border, tolerance).Count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Arc - Circle

        private IEnumerable<Vector2> InterceptCircle(Circle2 circle, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();

            var possibles = this.ToCircle().Intersect(circle, tolerance);
            foreach (var p in possibles)
            {
                if (this.Contains(p, tolerance))
                {
                    intersections.Add(p);
                }
            }

            return intersections;
        }

        private bool InterceptCircleWith(Circle2 circle, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var possibles = this.ToCircle().Intersect(circle, tolerance);
            foreach (var p in possibles)
            {
                if (this.Contains(p, tolerance)) return true;
                    
            }
            return false;
        }


        #endregion

        #region Arc - Arc

        private IEnumerable<Vector2> InterceptArc(Arc arc, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersections = new List<Vector2>();
            var c1 = this.ToCircle();
            var c2 = arc.ToCircle();

            var possibles = c1.Intersect(c2, tolerance);

            foreach (var p in possibles)
            {
                if (this.Contains(p, tolerance) && arc.Contains(p, tolerance))
                    intersections.Add(p);
            }


            return intersections;
        }

        private bool InterceptArcWith(Arc arc, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var c1 = this.ToCircle();
            var c2 = arc.ToCircle();
            var possibles = c1.Intersect(c2, tolerance);

            foreach (var p in possibles)
            {
                if (this.Contains(p, tolerance) && arc.Contains(p, tolerance))
                    return true;
            }

            return false;
        }


        #endregion

        public bool HasCollision(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (other is LineSegment2)
            {
                return this.InterceptLineWith(other as LineSegment2, tolerance);
            }
            else if (other is Circle2)
            {
                return this.InterceptCircleWith(other as Circle2, tolerance);
            }
            else if (other is Rectangle2)
            {
                return this.InterceptWithPolygon(((Rectangle2)other).ToPolygon2(), tolerance);
            }
            else if (other is Arc)
            {
                return this.InterceptArcWith(other as Arc, tolerance);
            }
            else
            {
                return other.HasCollision(this, tolerance);
            }
        }

        public IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var pnts = new List<Vector2>();

            if (other is LineSegment2)
            {
                pnts.AddRange(this.InterceptLine(other as LineSegment2, tolerance));
            }
            else if (other is Rectangle2)
            {
                pnts.AddRange(this.InterceptPolygon(((Rectangle2)other).ToPolygon2(), tolerance));
            }
            else if (other is Polygon2)
            {
                pnts.AddRange(this.InterceptPolygon(other as Polygon2, tolerance));
            }
            else if (other is Circle2)
            {
                pnts.AddRange(this.InterceptCircle(other as Circle2, tolerance));
            }
            else if (other is Arc)
            {
                pnts.AddRange(this.InterceptArc(other as Arc, tolerance));
            }
            else
            {
                if (other is IShape)
                    pnts.AddRange(((IShape)other).ToPolygon2().Intersect(this, tolerance));
            }
            return pnts;
        }



        /// <summary>
        /// Does a Point lie on the Arc line?
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            bool conatins = false;

            // First, the distance from middlepoint to our Point must be equal to the radius:
            var distance = LineSegment2.CalcLenght(this.MiddlePoint, point);
            if (Math.Abs(distance - this.Radius) < tolerance)
            {
                // If this is true, we only need to check if we are in the arc angle

                var bowMiddle = this.GetPointOnArc(this.Angle/2);
                var l1 = new LineSegment2(this.Location, bowMiddle);
                var l2 = new LineSegment2(this.GetPointOnArc(this.Angle), bowMiddle);
                var intersection = new LineSegment2(this.MiddlePoint, point);
                conatins = intersection.InterceptLineWith(l1, tolerance) || intersection.InterceptLineWith(l2, tolerance);

            }
            return conatins;
        }
    }
}
