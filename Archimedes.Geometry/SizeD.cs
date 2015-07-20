using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Represents size (width and height) information
    /// </summary>
    public struct SizeD
    {
        #region Fields

        public readonly double Width;
        public readonly double Height;

        #endregion

        #region Static Builder

        public static readonly SizeD Empty = new SizeD(0, 0);

        /// <summary>
        /// Parses two numbers, delemited by a comma, to a Size structure
        /// </summary>
        /// <param name="sizeStr"></param>
        /// <returns></returns>
        public static SizeD Parse(string sizeStr)
        {
            double[] doubles = Parser.ParseItem2D(sizeStr);
            return new SizeD(doubles[0], doubles[1]);
        }

        #endregion

        #region Constructor

        public SizeD(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        public bool IsEmpty { get { return Width == 0 && Height == 0; } }

        #region Equality Members

        public override bool Equals(Object obj)
        {
            return obj is SizeD && this == (SizeD)obj;
        }
        public override int GetHashCode()
        {
            return Width.GetHashCode() * 17 ^ Height.GetHashCode();
        }
        public static bool operator ==(SizeD x, SizeD y)
        {
            return x.Width == y.Width && x.Height == y.Height;
        }
        public static bool operator !=(SizeD x, SizeD y)
        {
            return !(x == y);
        }

        #endregion

        public override string ToString()
        {
            return "{Width=" + Width.ToString((IFormatProvider)CultureInfo.CurrentCulture) + ", Height=" + Height.ToString((IFormatProvider)CultureInfo.CurrentCulture) + "}";
        }

    }
}
