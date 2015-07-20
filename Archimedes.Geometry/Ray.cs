namespace Archimedes.Geometry
{
    /// <summary>
    /// A Ray is a one-direction infinite Line.
    /// </summary>
    public class Ray
    {
        #region Fields

        private Line _infiniteLine;

        #endregion

        #region Constructors

        public Ray(Vector2 v, Vector2 startpnt)
            : this(v.X, v.Y, startpnt) { }

        public Ray(Ray prototype)
            : this(prototype.Direction, prototype.Location) { }

        /// <summary>
        /// Creates a new Ray
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="startPoint">The origin location of this ray</param>
        public Ray(double x, double y, Vector2 startPoint) {
            _infiniteLine = new Line(startPoint, new Vector2(x, y));
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Startpoint of this Ray
        /// </summary>
        public Vector2 Location {
            get { return _infiniteLine.Location; }
            set { _infiniteLine.Location = value; }
        }

        /// <summary>
        /// Vector of this Ray
        /// </summary>
        public Vector2 Direction {
            get { return _infiniteLine.Direction; }
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Test if a Point lies on the Ray
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (_infiniteLine.Contains(point))
            {
                var v = new Vector2(Location, point);
                return (Direction.IsDirectionEqual(v, tolerance));
            }
            return false;
        }
        
        public Ray Clone() {
            return new Ray(this);
        }
        #endregion

        #region Intersection Ray - Ray

        public Vector2 Intersect(Ray uRay, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var point = IntersectN(uRay, tolerance);
            if (point.HasValue)
                return point.Value;
            else
                return Vector2.Zero;
        }

        public bool IntersectWith(Ray uRay, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var point = IntersectN(uRay, tolerance);
            return point.HasValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uRay"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private Vector2? IntersectN(Ray uRay, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var intersection = _infiniteLine.Intersection(uRay._infiniteLine, tolerance);

            if (intersection.HasValue)
            {
                //check if computed point lies on our line.
                if (Contains(intersection.Value, tolerance) || uRay.Contains(intersection.Value, tolerance))
                {
                    return intersection;
                }
            }
            return null;
        }

        #endregion


    }
}
