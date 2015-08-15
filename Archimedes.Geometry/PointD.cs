using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Represents a point where the components are of type double
    /// </summary>
    public struct PointD : IEquatable<PointD>, IOrdered<PointD>
    {
        public readonly double X;
        public readonly double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        #region IEquatable

        public bool Equals(PointD other)
        {
            return this.Y == other.Y && this.X == other.X;
        }

        public bool Equals(PointD other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return Math.Abs(other.X - this.X) < tolerance &&
                   Math.Abs(other.Y - this.Y) < tolerance;
        }

        public override bool Equals(object obj)
        {
            if (obj is PointD)
            {
                return Equals((PointD)obj);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        #endregion

        #region IOrdered

        /// <summary>
        /// Lexically check if the given Vector is less than this one
        /// </summary>
        /// <param name="p2"></param>
        /// <returns></returns>
        public bool Less(PointD p2)
        {
            return X < p2.X || X == p2.X && Y < p2.Y;
        }

        #endregion
    }
}
