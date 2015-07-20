using System;
using System.Collections.Generic;
using System.Linq;
using Archimedes.Geometry.Algorithms;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// Represents a closed 2D Area.
    /// </summary>
    public partial class Polygon2 : IShape
    {
        #region Fields

        private Vertices _vertices = new Vertices();
        private bool _boundCircleChanged = false;

        private Vector2 _middlePoint;
        private Circle2 _boundingCircle;

        #endregion

        /// <summary>
        /// Creates a new polygon from the given parsed vertices
        /// </summary>
        /// <param name="polygonStr"></param>
        /// <returns></returns>
        public static Polygon2 Parse(string polygonStr)
        {
            return new Polygon2(Vector2.ParseAll(polygonStr));
        }

        #region Constructor

        public Polygon2() { }

        public Polygon2(IEnumerable<Vector2> vertices) {
            this.AddRange(vertices);
        }

        public Polygon2(Polygon2 prototype) {
            this.Prototype(prototype);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Vertices count in this polygon
        /// </summary>
        public int VerticesCount
        {
            get { return _vertices != null ? _vertices.Count : 0; }
        }

        #endregion

        #region Public Vertex Access Methods

        public virtual void Clear() {
            _vertices.Clear();
            Invalidate();
        }

        public virtual void Add(Vector2 uvertex)
        {
            _vertices.Add(uvertex);
            Invalidate();
        }
        public virtual void Remove(Vector2 uvertex)
        {
            _vertices.Remove(uvertex);
            Invalidate();
        }

        public virtual void AddRange(IEnumerable<Vector2> uvertices)
        {
            _vertices.AddRange(uvertices);
            Invalidate();
        }

        /// <summary>
        /// Direct access to the vertices of this polygon
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 this[int index]
        {
            get
            {
                return _vertices[index];
            }
            set
            {
                _vertices[index] = value;
                Invalidate();
            }
        }

        #endregion

        #region Prototype Methods

        public void Prototype(IGeometry iprototype) {
            var prototype = iprototype as Polygon2;
            if (prototype == null)
                throw new NotSupportedException();

            this.AddRange(prototype.ToVertices());
        }

        #endregion

        #region Public Methods

        #region Orientation Routines

        /// <summary>
        /// Return True if the polygon is oriented clockwise.
        /// </summary>
        /// <returns></returns>
        public bool IsOrientedClockwise() {
            return (SignedPolygonArea() < 0);
        }


        /// <summary>
        /// If the polygon is oriented counterclockwise, reverse the order of its points.
        /// </summary>
        public void OrientClockwise() {
            if (!IsOrientedClockwise())
                _vertices = new Vertices(_vertices.Reverse());
        }

        /// <summary>
        /// If the polygon is oriented clockwise, reverse the order of its points.
        /// </summary>
        public void OrientCounterClockwise() {
            if (IsOrientedClockwise())
                _vertices = new Vertices(_vertices.Reverse());
        }

        #endregion // Orientation Routines

        #region Area Routines

        /// <summary>
        /// Return the polygon's area in "square units."
        /// Add the areas of the trapezoids defined by the
        /// polygon's edges dropped to the X-axis. When the
        /// program considers a bottom edge of a polygon, the
        /// calculation gives a negative area so the space
        /// between the polygon and the axis is subtracted,
        /// leaving the polygon's area. This method gives odd
        /// results for non-simple polygons.
        /// </summary>
        /// <returns></returns>
        public double Area
        {
            get {
                // Return the absolute value of the signed area.
                // The signed area is negative if the polyogn is
                // oriented clockwise.
                return Math.Abs(SignedPolygonArea());
            }
        }


        /// <summary>
        /// Return the polygon's area in "square units."
        /// Add the areas of the trapezoids defined by the
        /// polygon's edges dropped to the X-axis. When the
        /// program considers a bottom edge of a polygon, the
        /// calculation gives a negative area so the space
        /// between the polygon and the axis is subtracted,
        /// leaving the polygon's area. This method gives odd
        /// results for non-simple polygons.
        ///
        /// The value will be negative if the polyogn is
        /// oriented clockwise.
        /// </summary>
        /// <returns></returns>
        private double SignedPolygonArea()
        {
            // Add the first point to the end.
            int num_points = _vertices.Count;
            if (num_points == 0)
                return 0;
            var pts = new Vector2[num_points + 1];
            _vertices.CopyTo(pts, 0);
            pts[num_points] = _vertices[0];

            // Get the areas.
            double area = 0;
            for (int i = 0; i < num_points; i++) {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return area;
        }
        #endregion // Area Routines

        #region Convex Methods

        /// <summary>
        /// Determites if this polygon is convex.
        /// </summary>
        /// <returns>Return True if the polygon is convex.</returns>
        public bool IsConvex() {
            CleanUp();
            // For each set of three adjacent points A, B, C,
            // find the dot product AB · BC. If the sign of
            // all the dot products is the same, the angles
            // are all positive or negative (depending on the
            // order in which we visit them) so the polygon
            // is convex.
            bool got_negative = false;
            bool got_positive = false;
            int num_points = _vertices.Count;
            int B, C;
            for (int A = 0; A < num_points; A++) {
                B = (A + 1) % num_points;
                C = (B + 1) % num_points;

                var crossProduct =
                    CrossProductLength(
                        _vertices[A].X, _vertices[A].Y,
                        _vertices[B].X, _vertices[B].Y,
                        _vertices[C].X, _vertices[C].Y);
                if (crossProduct < 0) {
                    got_negative = true;
                } else if (crossProduct > 0) {
                    got_positive = true;
                }
                if (got_negative && got_positive) return false;
            }
            return true;
        }

        #endregion

        #endregion

        #region Centroid/Middlepoint
        /// <summary>
        /// Get the middlepoint of this polygon
        /// </summary>
        /// <returns></returns>
        public Vector2 MiddlePoint {
            get {
                if(_boundCircleChanged) {
                    _middlePoint = FindCentroid();
                }
                return _middlePoint;
            }
            set {
                var move = new Vector2(this.MiddlePoint, value);
                this.Translate(move);
            }
        }

        /// <summary>
        /// Find the polygon's centroid.
        /// </summary>
        /// <returns></returns>
        private Vector2 FindCentroid() {

            if (_vertices.Count == 0)
                return Vector2.Zero; // No center if there are no vertices

            var polygonArea = this.Area;
            if (polygonArea == 0)
                return Vector2.Zero; // No center if there is no Area

            // Add the first point at the end of the array.
            int numPoints = _vertices.Count;
            var pts = new Vector2[numPoints + 1];
            _vertices.CopyTo(pts, 0);
            pts[numPoints] = _vertices[0];

            // Find the centroid.
            double x = 0;
            double y = 0;
            for (var i = 0; i < numPoints; i++)
            {
                var secondFactor = pts[i].X*pts[i + 1].Y -
                                   pts[i + 1].X*pts[i].Y;
                x += (pts[i].X + pts[i + 1].X)*secondFactor;
                y += (pts[i].Y + pts[i + 1].Y)*secondFactor;
            }

            // Divide by 6 times the polygon's area.
            x /= (6 * polygonArea);
            y /= (6 * polygonArea);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (x < 0) {
                x = -x;
                y = -y;
            }
            return new Vector2(x, y);
        }

        #endregion

        #region Public Transformator Methods

        /// <summary>
        /// Rotates the Polygon around the origin. If no Origin is specified, the middlepoint is taken by default.
        /// </summary>
        /// <param name="angle">Rotation to rotate</param>
        /// <param name="origin">Point to rotate around. Default is middlepoint of this polygon</param>
        public void Rotate(Angle angle, Vector2? origin = null) {
            _vertices = _vertices.RotateVertices(origin ?? this.MiddlePoint, angle);
            Invalidate();
        }


        /// <summary>
        /// Moves the Polygon (each vertex) along the given Vector
        /// </summary>
        /// <param name="v"></param>
        public void Translate(Vector2 v) {
            for(int i=0; i < _vertices.Count; i++) {
                _vertices[i] = _vertices[i] + v;
            }
            Invalidate();
        }

        public void Scale(double fact) {
            for (var i = 0; i < _vertices.Count; i++) {
                _vertices[i] = _vertices[i].Scale(fact);
            }
            Invalidate();
        }

        #endregion

        #region To Methods

        /// <summary>
        /// Gets all border lines of this polygon including the closing line
        /// </summary>
        /// <returns></returns>
        public LineSegment2[] ToLines()
        {
            var lines = new LineSegment2[_vertices.Count];

            if(_vertices.Count > 1)
            {
                for (int i = 0; i < _vertices.Count; i++)
                {
                    lines[i] = new LineSegment2(
                        _vertices[i], 
                        _vertices[(i + 1) % _vertices.Count] // next vertex, cycle on end to the beginning
                        );
                }
            }
            return lines;
        } 


        /// <summary>
        /// Returns this Polygon as convex polygon.
        /// 
        /// If this polygon is convex, a clone of it is returned.
        /// If this polygon is concave, the Graham's (1972) "points to convex hull" enhanched algorythm is used to generate a convex polygon hull.
        /// </summary>
        /// <returns></returns>
        public Polygon2 ToConvexPolygon() {

            var convexhull = ConvexHullBuilder.Convexhull(this.ToVertices().Distinct());

            var clone = this.Clone() as Polygon2;
            clone.Clear();
            clone.AddRange(convexhull.ToVertices());
            return clone;
        }

        public Vertices ToVertices() {
            return new Vertices(_vertices);
        }

        public Polygon2 ToPolygon2() {
            return this;
        }

        #endregion

        #region IGeometryBase

        public Vector2 Location {
            get { return this.MiddlePoint; }
            set { this.MiddlePoint = value; }
        }


        public IGeometry Clone() {
            return new Polygon2(this);
        }

        /// <summary>
        /// Gets the BoundingBox (Axis Parallel) of this Polygon
        /// </summary>
        public AARectangle BoundingBox
        {
            get { return _vertices.BoundingBox; }
        }

        #endregion

        #region Cross and Dot Products
        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        private static double CrossProductLength(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            var BAx = Ax - Bx;
            var BAy = Ay - By;
            var BCx = Cx - Bx;
            var BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        #endregion // Cross and Dot Products

        public virtual void Dispose() {
            //Pen.Dispose();
            //FillBrush.Dispose();
        }

        private void Invalidate() {
            _boundCircleChanged = true;
        }

        private void CleanUp()
        {
            _vertices = new Vertices(_vertices.Distinct());
        }

        public override string ToString()
        {
            return string.Format("VerticesCount: {0}, Location: {1}, Vertices: {2}", VerticesCount, Location, _vertices);
        }
    }
}
