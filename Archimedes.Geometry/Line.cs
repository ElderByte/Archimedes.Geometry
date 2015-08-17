using System;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Represents an invinite line in 2d space.
    /// 
    /// Line equation:  y = m * x + c
    /// 
    /// m : Slope
    /// c : y-intercept
    /// 
    /// </summary>
    public class Line
    {
        #region Fields
        
        private Vector2 _location;
        private Vector2 _direction;

        #endregion

        #region Constructors

        public Line(Vector2 location, Vector2 direction)
        {
            _location = location;
            _direction = direction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A point on this line - which defines the location of it.
        /// </summary>
        public Vector2 Location
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// The line direction
        /// </summary>
        public Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Gets the slope of this line. If this line is vertical, the slope is defined as ZERO.
        /// </summary>
        public double Slope
        {
            get { return !Direction.IsVertical ? Direction.Slope : 0; }
        }

        public bool IsVertical
        {
            get { return Direction.IsVertical; }
        }

        public bool IsHorizontal
        {
            get { return Direction.IsHorizontal; }
        }

        /// <summary>
        /// Returns the Y coordinate where the line intersects the Y-Axis.
        /// If this line is vertical, return ZERO.
        /// </summary>
        /// <returns></returns>
        public double IntersectY
        {
            get
            {
                if (this.IsVertical) return 0.0; // Never a proper intersection
                return -Slope * Location.X + Location.Y;
            }
        }


        #endregion

        /// <summary>
        /// Checks if this line is parallel to the given one
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsParallelTo(Line other)
        {
            return Direction.IsParallelTo(other.Direction);
        }

        /// <summary>
        /// Checks if the given point lies on this infinite line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (Direction.IsVertical)
            {
                // This line is vertical - just check if the point has the same x value
                return Math.Abs(point.X - Location.X) < tolerance;
            }
            else
            {
                // We use the line equation y = m * x + c
                return (Math.Abs(point.Y - (Slope * point.X + IntersectY)) < tolerance);
            }
        }

        /// <summary>
        /// Returns the intersection point of this line with another one.
        /// If the lines are parallel, NULL is returned.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public Vector2? Intersection(Line other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {

            if (Direction.IsParallelTo(other.Direction, tolerance))
            {
                return null;
            }

            double intersectionpntX = 0;
            double intersectionpntY = 0;

            

            if (!Direction.IsVertical && !other.Direction.IsVertical)
            {    // both NOT vertical
                intersectionpntX = ((-1.0 * (this.IntersectY - other.IntersectY)) / (Direction.Slope - other.Direction.Slope));
                intersectionpntY = (Direction.Slope * intersectionpntX + this.IntersectY);
            }
            else if (Direction.IsVertical)
            {                  // this vertical (so it must lie on this.X)
                intersectionpntX = Location.X;
                intersectionpntY = (other.Direction.Slope * intersectionpntX + other.IntersectY);
            }
            else if (other.Direction.IsVertical)
            {                // Line2 vertical (so it must lie on Line2.X)
                intersectionpntX = other.Location.X;
                intersectionpntY = (Direction.Slope * intersectionpntX + this.IntersectY);
            }

            return new Vector2(intersectionpntX, intersectionpntY);

        }

     
    }
}
