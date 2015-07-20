using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                var m = Slope;
                var c = CalcYIntercept();
                return (Math.Abs(point.Y - m * point.X + c) < tolerance);
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
                intersectionpntX = ((-1.0 * (this.CalcYIntercept() - other.CalcYIntercept())) / (Direction.Slope - other.Direction.Slope));
                intersectionpntY = (Direction.Slope * intersectionpntX + this.CalcYIntercept());
            }
            else if (Direction.IsVertical)
            {                  // this vertical (so it must lie on this.X)
                intersectionpntX = Location.X;
                intersectionpntY = (other.Direction.Slope * intersectionpntX + other.CalcYIntercept());
            }
            else if (other.Direction.IsVertical)
            {                // Line2 vertical (so it must lie on Line2.X)
                intersectionpntX = other.Location.X;
                intersectionpntY = (Direction.Slope * intersectionpntX + this.CalcYIntercept());
            }

            return new Vector2(intersectionpntX, intersectionpntY);

        }

        #region Private methods

        /// <summary>
        /// Get the c variable value in line equation.
        /// This value is the y-intersection of this line.
        /// c = y - m * x
        /// </summary>
        /// <returns></returns>
        private double CalcYIntercept()
        { 
            if (Direction.IsVertical)
            {
                return 0;
            }
            else
            {
                return Location.Y - (Slope * Location.X);
            }
        }



        #endregion
    }
}
