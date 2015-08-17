/*******************************************
 * 
 *  Written by Pascal Büttiker (c)
 *  April 2010
 * 
 * *****************************************
 * *****************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// Represents a Line segment in a 2D coord space.
    /// A line segment is a part of a line that is bounded by two distinct end points, and contains every point on the line between its end points.
    /// </summary>
    public partial class LineSegment2 : IGeometry, IEquatable<LineSegment2>
    {
        
        #region Fields

        Vector2 _start;
        Vector2 _end;

        #endregion

        #region Static Builder

        /// <summary>
        /// Parses the given vertices string into a line.
        /// Expected format: "(x1,y1),(x2,y2)"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LineSegment2 Parse(string value)
        {
            LineSegment2 line;
            var vertices = Vector2.ParseAll(value);
            if (vertices.Length == 2)
            {
                line = new LineSegment2(vertices[0], vertices[1]);
            }
            else
            {
                throw new ArgumentOutOfRangeException("value", 
                    "Expected where 2 vertices to build a line, but got: " +vertices.Length + " - Parsed from '" + value+"'");
            }
            return line;
        }


        /// <summary>
        /// Explodes a Rectangle into the 4 border lines and returns an array of lines
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>4 lines, one for each side of the rectangle</returns>
        public static LineSegment2[] FromRectangle(AARectangle rect)
        {
            var topLeft = rect.Location;
            var topRight = new Vector2(rect.X + rect.Width, rect.Y);
            var bottomRight = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
            var bottomLeft = new Vector2(rect.X, rect.Y + rect.Height);

            return new[]
            {
                new LineSegment2(topLeft, topRight),
                new LineSegment2(topRight, bottomRight),
                new LineSegment2(bottomRight, bottomLeft),
                new LineSegment2(bottomLeft, topLeft)
            };
        }

        /// <summary>
        /// Buils lines from the given vertices
        /// </summary>
        /// <param name="vertices">The vertices - they must be ordered as the lines shall be created</param>
        /// <returns></returns>
        public static IEnumerable<LineSegment2> FromVertices(Vertices vertices)
        {
            if (vertices.Count > 1)
            {
                for (int i = 0; i < vertices.Count - 1; i++)
                {
                    yield return new LineSegment2(vertices[i], vertices[i + 1]);
                }
            }
        }

        #endregion

        #region Constructors


        /// <summary>
        /// Creates a new horizontal Line, starting from 0,0 with the given Length
        /// </summary>
        /// <param name="lenght">Lenght of the new Line</param>
        public LineSegment2(double lenght) {
            _start = Vector2.Zero;
            _end = new Vector2(lenght, 0);
        }

        /// <summary>
        /// Creates a Line between two given Points represented by Vectors
        /// </summary>
        /// <param name="start">Startpoint of the Line</param>
        /// <param name="end">Endpoint of the Line</param>
        public LineSegment2(Vector2 start, Vector2 end)
        {
            _start = start;
            _end = end;
        }

        public LineSegment2(double uP1x, double uP1y, double uP2x, double uP2y)
        {
            _start = new Vector2(uP1x, uP1y);
            _end = new Vector2(uP2x, uP2y);
        }

        public LineSegment2(LineSegment2 prototype) {
            Prototype(prototype);
        }

        public virtual void Prototype(IGeometry iprototype) {
            var prototype = iprototype as LineSegment2;
            if (prototype == null)
                throw new NotImplementedException();

            this.Start = prototype.Start;
            this.End = prototype.End;
        }

        #endregion

        #region Public Propertys

        /// <summary>
        /// Get/Set the Startpoint of this Line
        /// </summary>
        public Vector2 Start {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Get/Set the Endpoint of this Line
        /// </summary>
        public Vector2 End {
            get { return _end; }
            set { _end = value; }
        }

        /// <summary> 
        /// Return the q (movement from x axis). If slope isn't defined, this property is Zero.
        /// </summary>
        public double YMovement {
            get {   // q = y1 - m * x1 
                if (this.IsVertical) {
                    return 0;
                }
                else {
                    return this.Start.Y - (this.Slope * this.Start.X);
                }
            }
        }

        /// <summary>
        /// Returns the solpe of the line. Returns Zero if the slope isn't defined.
        /// </summary>
        public double Slope {
            get
            {
                return ToVector().Slope;
            }
        }

        /// <summary>
        /// Extends this line in the given direction
        /// </summary>
        /// <param name="len"></param>
        /// <param name="direction"></param>
        public void Stretch(double len, Direction direction){
            var vThis = ToVector();

            if (direction == Direction.RIGHT) {
                vThis = vThis.WithLength(vThis.Length + len);
                this.End = this.Start + vThis;
            } else
            {
                vThis = vThis.WithLength(len) * -1;
                this.Start += vThis;
            }
        }

        /// <summary>
        /// Returns the lenght of this line
        /// </summary>
        public double Length {
            get { return CalcLenght(this.Start, this.End); }
        }

        /// <summary>
        /// Determites if the given Point/Vertex lies on the Left side of this Line
        /// </summary>
        /// <param name="point">The point to test</param>
        /// <returns></returns>
        public bool IsLeft(Vector2 point) {
            return IsLeft(this.Start, this.End, point);
        }

        /// <summary>
        /// Calculates if the given point is on the left side of the given line
        /// </summary>
        /// <param name="start">Start point of the line</param>
        /// <param name="end">End point of the line</param>
        /// <param name="point">The point to test</param>
        /// <returns></returns>
        static bool IsLeft(Vector2 start, Vector2 end, Vector2 c)
        {
            return ((end.X - start.X) * (c.Y - start.Y) - (end.Y - start.Y) * (c.X - start.X)) > 0;
        }

        /// <summary>
        /// Calculates the distance between two Points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double CalcLenght(Vector2 start, Vector2 end) { // TODO Rename to distance()
            return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
        }
      

        /// <summary>
        /// Compare the slope of two lines  
        /// </summary>
        /// <param name="line"></param>
        /// <param name="tolerance"></param>
        /// <returns>true, if the solpes are equal</returns>
        public bool IsParallelTo(LineSegment2 line, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            return ToVector().IsParallelTo(line.ToVector(), tolerance);
        }

        /// <summary>
        /// Is this Line vertical?
        /// </summary>
        public bool IsVertical {
            get
            {
                return ToVector().IsVertical;
            }
        }

        /// <summary>
        /// Is this Line horizontal?
        /// </summary>
        public bool IsHorizontal {
            get
            {
                return ToVector().IsHorizontal;
            }
        }

        /// <summary>
        /// Is this Line horizontal or vertical?
        /// </summary>
        public bool IsHorOrVert {
            get {
                return this.IsHorizontal || this.IsVertical;
            }
        }


        #endregion

        #region Specail Transformators

        /// <summary>
        /// Scale the line by given scale factor
        /// </summary>
        /// <param name="factor">Scale Factor</param>
        public void Scale(double factor) {
            this.Start = Start.Scale(factor);
            this.End = End.Scale(factor);
        }

        /// <summary>
        /// Turns this line into a vector
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector() {
            return new Vector2(this.Start, this.End);
        }

        /// <summary>
        /// Turns this line into a series of vertices.
        /// </summary>
        /// <returns></returns>
        public Vertices ToVertices() {
            return new Vertices{ this.Start, this.End };
        }

        /// <summary>
        /// Returns an infinite line based on this line segment
        /// </summary>
        /// <returns></returns>
        public Line ToLine()
        {
            return new Line(Location, ToVector());
        }

        #endregion

        #region Geomerty Base

        public Vector2 Location {
            get {
                return this.Start;
            }
            set {
                Translate(new Vector2(this.Start, value));
            }
        }

        /// <summary>
        /// Gets the point in the middle of this line
        /// </summary>
        public Vector2 MiddlePoint {
            get {
                var toMiddle = ToVector() / 2;
                return Start + toMiddle;
            }
            set {
                Translate(new Vector2(this.MiddlePoint, value));
            }
        }

        public void Translate(Vector2 mov) {
            this.Start += mov;
            this.End += mov;
        }

        /// <summary>
        /// Creates a new Line, clone of the origin but moved by given vector
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public static LineSegment2 CreateMoved(LineSegment2 origin, Vector2 move) {
            var clone = origin.Clone() as LineSegment2;
            clone.Translate(move);
            return clone;
        }


        public IGeometry Clone() {
            return new LineSegment2(this);
        }

        public AARectangle BoundingBox {
	        get { 
                Vector2 start;
                if (Start.X > End.X) {
                    start = Start;
                }else if (Start.X == End.X){
                    if(Start.Y < End.Y)
                        start = Start;
                    else
                        start = End;
                }else
                    start = End;

                return new AARectangle(start, new SizeD(Math.Abs(Start.X - End.X), Math.Abs(Start.Y - End.Y)));
            }
        }

        public Circle2 BoundingCircle {
            get { 
                var m = this.MiddlePoint;
                return new Circle2(m, CalcLenght(m, this.Start));
            }
        }


        #endregion

        #region Geomtry Base Collision


        public IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE) {
            var pnts = new List<Vector2>();

            if (other is LineSegment2) {
                var pnt = this.IntersectLine(other as LineSegment2, tolerance);
                if(pnt.HasValue)
                    pnts.Add(pnt.Value);
            } else
                other.Intersect(this);
            return pnts;
        }

        
        public bool HasCollision(IGeometry geometry, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (geometry is LineSegment2)
            {
                var line = geometry as LineSegment2;

                if (HasOverlap(line, tolerance))
                {
                    return true;
                }
                else
                {
                    return this.HasIntersection(line, tolerance);
                }

            }else { //delegate Collision Detection to other Geometry Object
                return geometry.HasCollision(this, tolerance);
            }
        }


        public bool HasOverlap(LineSegment2 other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var overlapSegment = GetOverlapSegment(other, tolerance);
            return overlapSegment != null;
        }




        public LineSegment2 GetOverlapSegment(LineSegment2 line2, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            bool isHorizontal = this.IsHorizontal;
            bool isDescending = this.Slope < 0 && !isHorizontal;
            double invertY = isDescending || isHorizontal ? -1 : 1;

            var min1 = new Vector2(Math.Min(this.Start.X, this.End.X), Math.Min(this.Start.Y * invertY, this.End.Y * invertY));
            var max1 = new Vector2(Math.Max(this.Start.X, this.End.X), Math.Max(this.Start.Y * invertY, this.End.Y * invertY));

            var min2 = new Vector2(Math.Min(line2.Start.X, line2.End.X), Math.Min(line2.Start.Y * invertY, line2.End.Y * invertY));
            var max2 = new Vector2(Math.Max(line2.Start.X, line2.End.X), Math.Max(line2.Start.Y * invertY, line2.End.Y * invertY));

            Vector2 minIntersection;
            if (isDescending)
                minIntersection = new Vector2(Math.Max(min1.X, min2.X), Math.Min(min1.Y * invertY, min2.Y * invertY));
            else
                minIntersection = new Vector2(Math.Max(min1.X, min2.X), Math.Max(min1.Y * invertY, min2.Y * invertY));

            Vector2 maxIntersection;
            if (isDescending)
                maxIntersection = new Vector2(Math.Min(max1.X, max2.X), Math.Max(max1.Y * invertY, max2.Y * invertY));
            else
                maxIntersection = new Vector2(Math.Min(max1.X, max2.X), Math.Min(max1.Y * invertY, max2.Y * invertY));

            bool intersect = minIntersection.X <= maxIntersection.X &&
                             (!isDescending && minIntersection.Y <= maxIntersection.Y ||
                               isDescending && minIntersection.Y >= maxIntersection.Y);

            if (!intersect) return null;

            // Check if they only meet in a single point
            if (minIntersection.Equals(maxIntersection, tolerance)) return null;

            return new LineSegment2(minIntersection, maxIntersection);
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate smalles distance to a Target Point
        /// </summary>
        /// <param name="pt">Target Point</param>
        /// <param name="closest">Closest Point on Line to Targetpoint</param>
        /// <returns>Distance to target point</returns>
        public double FindDistanceToPoint(Vector2 pt, out Vector2 closest) {
            return FindDistanceToPoint(pt, this, out closest);
        }

        public double FindDistanceToPoint(Vector2 pt) {
            Vector2 dummy;
            return FindDistanceToPoint(pt, this, out dummy);
        }

        /// <summary>
        /// Calculate the closest distance between a given point pt and this line.
        /// </summary>
        /// <param name="pt">Target Point to wich distance is calculated</param>
        /// <param name="line">line</param>
        /// <param name="closest">closes Point on Line to target Point</param>
        /// <returns>Smallest distance to the targetpoint</returns>
        public static double FindDistanceToPoint(Vector2 pt, LineSegment2 line, out Vector2 closest) {
            var p1 = line.Start; var p2 = line.End;
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0)) {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0) {
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            } else if (t > 1) {
                closest = p2;
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            } else {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        #endregion

        public bool Equals(LineSegment2 other)
        {
            if (other == null) return false;
            return this.Start == other.Start && this.End == other.End;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}) : Length: {2}, Slope: {3}", Start, End, Length, Slope);
        }


    }
}
