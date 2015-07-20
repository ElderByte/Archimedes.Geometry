using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Units
{
    /// <summary>
    /// Represents a geometry angle which provides the angle value in common units.
    /// Specifically, degree and radian are suported units.
    /// </summary>
    [Serializable]
    public struct Angle : IComparable<Angle>, IEquatable<Angle>, IFormattable
    {
        private const double Tolerance = 0.00000001;

        /// <summary>
        /// The value in radians
        /// </summary>
        private readonly double _radians;

        #region Static Builder methods

        /// <summary>
        /// Zero angle, 0°
        /// </summary>
        public static readonly Angle Zero = new Angle(0, AngleUnit.Radians);

        /// <summary>
        /// A full rotation, equals to 360°
        /// </summary>
        public static readonly Angle FullRotation = new Angle(360, AngleUnit.Degrees);

        /// <summary>
        /// A half rotation, equals to 180°
        /// </summary>
        public static readonly Angle HalfRotation = new Angle(180, AngleUnit.Degrees);


        /// <summary>
        /// Creates a new instance of Rotation.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromRadians(double value)
        {
            return new Angle(value, AngleUnit.Radians);
        }

        /// <summary>
        /// Creates a new instance of Rotation.
        /// </summary>
        /// <param name="value"></param>
        public static Angle FromDegrees(double value)
        {
            return new Angle(value, AngleUnit.Degrees);
        }

        public static Angle Parse(string text)
        {
            return AngleParser.Parse(text);
        }

        /// <summary>
        /// Creates an angle with absolute value (Math.Abs())
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Angle Abs(Angle angle)
        {
            return Angle.FromRadians( Math.Abs(angle.Radians) );
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Rotation.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="unit"></param>
        public Angle(double value, AngleUnit unit)
        {
            switch (unit)
            {
                case AngleUnit.Degrees:
                    _radians = ConvertDegreesToRadians(value);
                    break;

                case AngleUnit.Radians:
                    _radians = value;
                    break;

                default:
                    throw new NotSupportedException("Unit " + unit + " is not supported!");
            }
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Returns the angle value in radians
        /// </summary>
        public double Radians
        {
            get
            {
                return _radians;
            }
        }

        /// <summary>
        /// Returns the angle value in degrees
        /// </summary>
        public double Degrees
        {
            get
            {
                return ConvertRadiansToDegrees(_radians);
            }
        }

        #endregion


        #region Public Methdos

        
        public double GetAngleAs(AngleUnit unit)
        {
            switch (unit)
            {
                    case AngleUnit.Radians:
                    return Radians;
                    case AngleUnit.Degrees:
                    return Degrees;
                default:
                    throw new NotSupportedException("Unit " +unit+ " is not supported!");
            }
        }

        public string GetShortName(AngleUnit unit)
        {
            switch (unit)
            {
                case AngleUnit.Radians:
                    return "rad";
                case AngleUnit.Degrees:
                    return "°";
                default:
                    throw new NotSupportedException("Unit " + unit + " is not supported!");
            }
        }

        public Angle Normalize()
        {
            return Angle.FromDegrees(Degrees % 360);
        }

        #endregion


        #region Public Static methods

        /// <summary>
        /// To convert a degrees value to radians, multiply it by pi/180 (approximately 0.01745329252).
        /// 
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double ConvertDegreesToRadians(double degree)
        {
            return degree * Math.PI / 180d;
        }

        /// <summary>
        ///  To convert a radians value to degrees, multiply it by 180/pi (approximately 57.29578).
        /// </summary>
        /// <param name="RAD"></param>
        /// <returns></returns>
        public static double ConvertRadiansToDegrees(double rad)
        {
            return rad * 180d / Math.PI;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Indicates whether two Rotation instances are equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.
        /// </returns>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether two Rotation instances are not equal.
        /// </summary>
        /// <returns>
        /// true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.
        /// </returns>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether a specified Rotation is less than another specified Rotation.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than the value of <paramref name="right"/>; otherwise, false. 
        /// </returns>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator <(Angle left, Angle right)
        {
            return left.Radians < right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified Rotation is greater than another specified Rotation.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than the value of <paramref name="right"/>; otherwise, false. 
        /// </returns>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static bool operator >(Angle left, Angle right)
        {
            return left.Radians > right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified Rotation is less than or equal to another specified Rotation.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is less than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An Rotation.</param>
        /// <param name="right">An Rotation.</param>
        public static bool operator <=(Angle left, Angle right)
        {
            return left.Radians <= right.Radians;
        }

        /// <summary>
        /// Indicates whether a specified Rotation is greater than or equal to another specified Rotation.
        /// </summary>
        /// <returns>
        /// true if the value of <paramref name="left"/> is greater than or equal to the value of <paramref name="right"/>; otherwise, false.
        /// </returns>
        /// <param name="left">An Rotation.</param>
        /// <param name="right">An Rotation.</param>
        public static bool operator >=(Angle left, Angle right)
        {
            return left.Radians >= right.Radians;
        }

        /// <summary>
        /// Multiplies an instance of Rotation with <paramref name="left"/> and returns the result.
        /// </summary>
        /// <param name="right">An instance of Rotation</param>
        /// <param name="left">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of Rotation with <paramref name="left"/> and returns the result.</returns>
        public static Angle operator *(double left, Angle right)
        {
            return Angle.FromRadians(left * right.Radians);
        }

        /// <summary>
        /// Multiplies an instance of Rotation with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of Rotation</param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Multiplies an instance of Rotation with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator *(Angle left, double right)
        {
            return Angle.FromRadians(left.Radians * right);
        }

        /// <summary>
        /// Divides an instance of Rotation with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <param name="left">An instance of Rotation</param>
        /// <param name="right">An instance of <seealso cref="T:System.Double"/></param>
        /// <returns>Divides an instance of Rotation with <paramref name="right"/> and returns the result.</returns>
        public static Angle operator /(Angle left, double right)
        {
            return Angle.FromRadians(left.Radians / right);
        }

        /// <summary>
        /// Adds two specified Rotation instances.
        /// </summary>
        /// <returns>
        /// An Rotation whose value is the sum of the values of <paramref name="left"/> and <paramref name="right"/>.
        /// </returns>
        /// <param name="left">A Rotation.</param>
        /// <param name="right">A TimeSpan.</param>
        public static Angle operator +(Angle left, Angle right)
        {
            return Angle.FromRadians(left.Radians + right.Radians);
        }

        /// <summary>
        /// Subtracts an angle from another angle and returns the difference.
        /// </summary>
        /// <returns>
        /// An Rotation that is the difference
        /// </returns>
        /// <param name="left">A Rotation (the minuend).</param>
        /// <param name="right">A Rotation (the subtrahend).</param>
        public static Angle operator -(Angle left, Angle right)
        {
            return Angle.FromRadians(left.Radians - right.Radians);
        }

        /// <summary>
        /// Returns an Rotation whose value is the negated value of the specified instance.
        /// </summary>
        /// <returns>
        /// An Rotation with the same numeric value as this instance, but the opposite sign.
        /// </returns>
        /// <param name="angle">A Rotation</param>
        public static Angle operator -(Angle angle)
        {
            return Angle.FromRadians(-1 * angle.Radians);
        }

        /// <summary>
        /// Returns the specified instance of Rotation.
        /// </summary>
        /// <returns>
        /// Returns <paramref name="angle"/>.
        /// </returns>
        /// <param name="angle">A Rotation</param>
        public static Angle operator +(Angle angle)
        {
            return angle;
        }

        #endregion

        #region ToString handling

        public override string ToString()
        {
            return this.ToString((string)null, (IFormatProvider)NumberFormatInfo.CurrentInfo);
        }

        public string ToString(string format)
        {
            return this.ToString(format, (IFormatProvider)NumberFormatInfo.CurrentInfo);
        }

        public string ToString(IFormatProvider provider)
        {
            return this.ToString((string)null, (IFormatProvider)NumberFormatInfo.GetInstance(provider));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString(format, formatProvider, AngleUnit.Degrees);
        }

        public string ToString(AngleUnit unit)
        {
            return ToString((string)null, (IFormatProvider)NumberFormatInfo.CurrentInfo, unit);
        }

        public string ToString(string format, AngleUnit unit)
        {
            return ToString(format, (IFormatProvider)NumberFormatInfo.CurrentInfo, unit);
        }

        public string ToString(string format, IFormatProvider formatProvider, AngleUnit unit)
        {
            var value = GetAngleAs(unit);
            return string.Format("{0}{1}", value.ToString(format, formatProvider), GetShortName(unit));
        }
        
        #endregion

        #region Equality members

        /// <summary>
        /// Compares this instance to a specified Rotation object and returns an integer that indicates whether this <see cref="instance"/> is shorter than, equal to, or longer than the Rotation object.
        /// </summary>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="value"/>.
        /// 
        ///                     Value
        /// 
        ///                     Description
        /// 
        ///                     A negative integer
        /// 
        ///                     This instance is smaller than <paramref name="value"/>.
        /// 
        ///                     Zero
        /// 
        ///                     This instance is equal to <paramref name="value"/>.
        /// 
        ///                     A positive integer
        /// 
        ///                     This instance is larger than <paramref name="value"/>.
        /// 
        /// </returns>
        /// <param name="value">A Rotation object to compare to this instance.</param>
        public int CompareTo(Angle value)
        {
            return this.Radians.CompareTo(value.Radians);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified Rotation object.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An Rotation object to compare with this instance.</param>
        public bool Equals(Angle other)
        {
            return Equals(other, Tolerance);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified Rotation object within the given tolerance.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> represents the same angle as this instance; otherwise, false.
        /// </returns>
        /// <param name="other">An Rotation object to compare with this instance.</param>
        /// <param name="tolerance">The maximum difference for being considered equal</param>
        public bool Equals(Angle other, double tolerance)
        {
            return Math.Abs(this.Radians - other.Radians) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is Angle && this.Equals((Angle)obj);
        }

        public override int GetHashCode()
        {
            return this.Radians.GetHashCode();
        }

        #endregion
    }
}
