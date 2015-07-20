/*******************************************
 * 
 *  Written by P. Büttiker
 *  
 *  Last updated: December, 2012 
 *  
 * *****************************************
 * *****************************************/
using System;
using System.Collections.Generic;
using Archimedes.Geometry.Extensions;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry
{
    /// <summary> 
    /// 2D Vector Type
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>, IOrdered<Vector2>
    {

        #region Static Builder methods

        /// <summary>
        /// Creates a Vector from the given angle and of the given length
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Vector2 FromAngleAndLenght(Angle angle, double length)
        {
            var gk = length * Math.Sin(angle.Radians);
            var ak = length * Math.Cos(angle.Radians);
            return new Vector2(ak, gk);
        }



        /// <summary>
        /// Parses the string to a vector
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector2 Parse(string value)
        {
            double[] doubles = Parser.ParseItem2D(value);
            return new Vector2(doubles);
        }

        public static Vector2[] ParseAll(string value)
        {
            var vertices = new List<Vector2>();
            var allNumbers = Parser.ParseItems2D(value);
            foreach (var d in allNumbers)
            {
                vertices.Add(new Vector2(d));
            }
            return vertices.ToArray();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new 2D Vector with given x/y Parts
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2(double x, double y)
            : this() {
            this.X = x;
            this.Y = y;
        }

        public Vector2(double[] data)
            : this(data[0], data[1])
        {
            if (data.Length != 2)
            {
                throw new ArgumentException("data.Length != 2!");
            }
        }

        /// <summary>
        /// Create a new 2D Vector from 2 given Points
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Vector2(Vector2 from, Vector2 to)
            : this(to.X - from.X, to.Y - from.Y) { }

        #endregion

        #region Static Properties

        /// <summary>
        /// Returns a Null-Vector
        /// </summary>
        public static readonly Vector2 Zero = new Vector2(0, 0); 

        /// <summary>
        /// Returns a X-Axis aligned Vector
        /// </summary>
        public static readonly Vector2 UnitX = new Vector2(1, 0);

        /// <summary>
        /// Returns a Y-Axis aligned Vector
        /// </summary>
        public static readonly Vector2 UnitY = new Vector2(0, 1);

        #endregion

        #region Public Propertys

        /// <summary>
        /// Gets the X Component of the Vector
        /// </summary>
        public readonly double X;

        /// <summary>
        /// Gets the Y Component of the Vector
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Gets the Lenght of this Vector
        /// </summary>
        public double Length
        {
            get { return (Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2))); }
        }

        /// <summary>
        /// Returns a new vector with the same direction but the given length
        /// </summary>
        /// <param name="newLenght"></param>
        /// <returns></returns>
        public Vector2 WithLength(double newLenght)
        {
            if (this.Length != 0)
            {
                //v * newsize / |v|
                return this*(newLenght/this.Length);
            }
            else
            {
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// Returns the slope of this Vector
        /// </summary>
        public double Slope
        {
            get { return (this.X != 0) ? (this.Y / this.X) : 0; }
        }

        public bool IsEmpty {
            get {
                return this == Vector2.Zero;
            }
        }

        #endregion

        #region Operator Overloads

        public static Vector2 operator *(double f1, Vector2 v1)
        {
            return new Vector2(v1.X * f1, v1.Y * f1);
        }

        public static Vector2 operator *(Vector2 v1, double f1)
        {
            return new Vector2(v1.X * f1, v1.Y * f1);
        }

        public static Vector2 operator /(Vector2 v1, double f1)
        {
            return new Vector2(v1.X / f1, v1.Y / f1);
        }

        /// <summary>
        /// Calc Scalarproduct (Dot-Product) of two Vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double operator *(Vector2 v1, Vector2 v2)
        {
            return v1.DotProduct(v2);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2) {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y); ;
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2) {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y); ;
        }

        public static bool operator ==(Vector2 v1, Vector2 v2){
            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2 v1, Vector2 v2) {
            return !(v1 == v2);
        }

        #endregion    

        #region Static Methods

        /// <summary>
        /// Multiplicate a Vector with a Scalar (Equal to v1[Vector] * Operator[Number])
        /// </summary>
        /// <param name="scalar"></param>
        public static Vector2 Multiply(Vector2 v, double scalar)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        /// <summary>
        /// Divides this Vector with a scalar
        /// </summary>
        /// <param name="scalar"></param>
        public Vector2 Divide(double scalar)
        {
            if (scalar != 0) {
                return new Vector2(this.X / scalar, this.Y / scalar);
            } else
                return Vector2.Zero;
        }

        /// <summary>
        ///  Calculates twice the signed area of the given triangle (p0, p1, p2)
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Area2(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            return p0.X * (p1.Y - p2.Y) + p1.X * (p2.Y - p0.Y) + p2.X * (p0.Y - p1.Y);
        }


        #endregion

        #region Transformation Methods

        /// <summary> 
        /// Calculates the Dot-Product between this and the given vector
        /// </summary>
        /// <param name="v2">Other Vector</param>
        /// <returns></returns>
        public double DotProduct(Vector2 v2)
        {
            return (this.X * v2.X) + (this.Y * v2.Y);
        }

        /// <summary>
        /// Rotates this Vector by the given amount
        /// </summary>
        /// <param name="angle"></param>
        public Vector2 GetRotated(Angle angle)
        {
            double r = this.Length;
            var thisAngle = angle + AngleSignedTo(Vector2.UnitX, true);
            return new Vector2(r * Math.Cos(thisAngle.Radians), r * Math.Sin(thisAngle.Radians));
        }

        /// <summary>
        /// Returns an Unit-Vector - a Vector with the same orientation but with 1 unit lenght
        /// </summary>
        /// <returns></returns>
        public Vector2 Normalize()
        {
            var len = this.Length;
            if (len != 0) // Avoid Division by Zero
            {
                return new Vector2(this.X/len, this.Y/len);
            }
            else
            {
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// Creates a scaled copy of this vector
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Vector2 Scale(double factor)
        {
            return this * factor;
        }

        #endregion

        #region Public Query Methods

        /// <summary>
        /// Checks if this vector is parallel to the given vector
        /// </summary>
        /// <param name="othervector"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool IsParallelTo(Vector2 othervector, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var @this = this.Normalize();
            var other = othervector.Normalize();
            var dp = Math.Abs(@this.DotProduct(other));
            return Math.Abs(1 - dp) < tolerance;
        }

        public bool IsPerpendicularTo(Vector2 othervector, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            var @this = this.Normalize();
            var other = othervector.Normalize();
            return Math.Abs(@this.DotProduct(other)) < tolerance;
        }

        public bool IsVertical {
            get { return (Math.Abs(this.X) < GeometrySettings.DEFAULT_TOLERANCE); }
        }

        public bool IsHorizontal {
            get { return (Math.Abs(this.Y) < GeometrySettings.DEFAULT_TOLERANCE); }
        }

        public bool IsDirectionEqual(Vector2 v2, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            bool bEqual = false;
            if (this.IsParallelTo(v2, tolerance))
            {
                bEqual = (this.X.IsSignEqual(v2.X) && this.Y.IsSignEqual(v2.Y));
            }
            return bEqual;
        }

        /// <summary>
        /// Get a point starting from this, with the given distance and angle
        /// </summary>
        /// <param name="distance">distance to the new point</param>
        /// <param name="angle">angle to the new point</param>
        /// <returns></returns>
        public Vector2 GetPoint(double distance, Angle angle)
        {
            var newPnt = new Vector2(
                this.X + (distance * Math.Cos(angle.Radians)),
                this.Y + (distance * Math.Sin(angle.Radians)));
            return newPnt;
        }


        /// <summary>
        /// Returns the angle to the X-Axis
        /// </summary>
        /// <returns></returns>
        public Angle GetAngleToXLegacy()
        {
            return GetAngleTo(Vector2.UnitX);
        }

        /// <summary>
        /// Returns the Rotation between two vectors in range of [0° - 180°] 
        /// (If you need 0° -360°, use GetAngleBetweenClockWise() instead.)
        /// </summary>
        /// <param name="toVector"></param>
        /// <returns></returns>
        public Angle GetAngleTo(Vector2 toVector)
        {
            var @this = this.Normalize();
            var other = toVector.Normalize();
            var angle = new Angle(Math.Acos(@this.DotProduct(other)), AngleUnit.Radians);

            if (angle > Angle.FromDegrees(180))
            { // From mathematic definition, it's always the shorter angle to return.
                angle = Angle.FromDegrees(360) - angle;
            }
            return angle;
        }




        /**/
        /// <summary>
        /// Returns the Rotation between this and the given vector in the range of [0-360°],
        /// excpet when return negative is true, then for angles > 180°, a negative value is returned
        /// 
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="clockWise">If true, calculate clock-wise</param>
        /// <param name="returnNegative">If angle is > 180° a negative value is returned.</param>
        /// <returns></returns>
        public Angle AngleSignedTo(Vector2 v2, bool clockWise, bool returnNegative = false)
        {
            int sign = clockWise ? -1 : 1;
            double a1 = Math.Atan2(this.Y, this.X);
            if (a1 < 0)
            {
                a1 += 2 * Math.PI;
            }

            double a2 = Math.Atan2(v2.Y, v2.X);
            if (a2 < 0)
            {
                a2 += 2 * Math.PI;
            }

            double a = sign * (a2 - a1);
            if (a < 0 && !returnNegative)
            {
                a += 2 * Math.PI;
            }

            if (a > Math.PI && returnNegative)
            {
                a -= 2 * Math.PI;
            }

            return new Angle(a, AngleUnit.Radians);
        }


        /*
        public Rotation GetAngle2V(Vector2 vbase)
        {
            double gamma;
            double tmp = this.DotProduct(vbase) / (this.Length * vbase.Length);
            gamma = Rotation.ConvertRadiansToDegrees(Math.Acos(tmp));
            if (gamma > 180)
            { //from mathematic definition, it's always the shorter angle to return.
                gamma = 360 - gamma;
            }
            return new Rotation(gamma, AngleUnit.Degrees);
        }

        public Rotation GetAngleBetweenClockWise(Vector2 b, Direction direction)
        {
            var theta = GetAngle2V(b);
            if (((this.Y * b.X - this.X * b.Y) > 0) == (direction == Direction.RIGHT))
            {
                return theta;
            }
            else
            {
                return Rotation.FromDegrees(360) - theta;
            }
        }*/
        

        /// <summary>
        /// Calculate a Vector which stands orthogonal on this Vector.
        /// </summary>
        /// <param name="direction">The desired Direction of the new orthogonal Vector</param>
        /// <returns></returns>
        public Vector2 GetOrthogonalVector(Direction direction)
        {
            // Crossproduct as unary operator (getting orthogonal)
            double orthVectorX, orthVectorY;
            if (direction == Direction.LEFT)
            {
                orthVectorX = this.Y * (-1);
                orthVectorY = this.X;
            }
            else
            { // RIGHT
                orthVectorX = this.Y;
                orthVectorY = this.X * (-1);
            }
            return new Vector2(orthVectorX, orthVectorY);
        }

        #endregion

        #region Common Methods

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }

        #endregion

        #region IEquatable

        public bool Equals(Vector2 other) {
            return this.Y == other.Y && this.X == other.X;
        }

        public bool Equals(Vector2 other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance;
        }

        public override bool Equals(object obj) {
            if (obj is Vector2) {
                return Equals((Vector2)obj);
            } else
                return false;
        }

        public override int GetHashCode() {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        #endregion

        #region IOrdered

        /// <summary>
        /// Lexically check if the given Vector is less than this one
        /// </summary>
        /// <param name="o2"></param>
        /// <returns></returns>
        public bool Less(IOrdered<Vector2> o2)
        {
            var p2 = (Vector2)o2;
            return X < p2.X || X == p2.X && Y < p2.Y;
        }

        #endregion
    }
}

