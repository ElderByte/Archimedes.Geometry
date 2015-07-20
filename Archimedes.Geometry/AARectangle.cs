using System;
using System.Globalization;
using Archimedes.Geometry.Primitives;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Represents an axis aligned rectangle / box.
    /// This class is immutable.
    /// </summary>
    public struct AARectangle
    {
        #region Fields

        private readonly double _x;
        private readonly double _y;
        private readonly double _width;
        private readonly double _height;

        #endregion

        #region Static builder methods

        public static readonly AARectangle Empty = new AARectangle(0, 0, 0, 0);

        /// <summary>
        /// Creates a rectangle that represents the intersetion between a and
        ///    b. If there is no intersection, null is returned.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static AARectangle Intersect(AARectangle a, AARectangle b)
        {
            double x1 = Math.Max(a.X, b.X);
            double x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            double y1 = Math.Max(a.Y, b.Y);
            double y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new AARectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Empty;
        }

        /// <summary>
        /// Creates a rectangle that represents the union between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static AARectangle Union(AARectangle a, AARectangle b)
        {
            double x1 = Math.Min(a.X, b.X);
            double x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            double y1 = Math.Min(a.Y, b.Y);
            double y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new AARectangle(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>
        ///  Creates a rectangle
        ///       that is inflated by the specified amount.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static AARectangle Inflate(AARectangle rect, float x, float y)
        {
            return new AARectangle(rect.X - x, rect.Y - y, rect.Width + 2 * x, rect.Height + 2 * y);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new axis aligned rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public AARectangle(double x, double y, double width, double height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Creates a new axis aligned rectangle
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public AARectangle(Vector2 location, SizeD size)
            :this(location.X, location.Y, size.Width, size.Height)
        {
        }

        #endregion

        #region Public Properties

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public Vector2 Location
        {
            get
            {
                return new Vector2(X, Y);
            }
        }

        public SizeD Size
        {
            get
            {
                return new SizeD(Width, Height);
            }
        }

        public Vector2 MiddlePoint
        {
            get
            {
                return new Vector2(
                    X + Width/2.0,
                    Y + Height/2.0);
            }
        }

        public double Area
        {
            get { return Width * Height; }
        }

        public Circle2 BoundingCircle
        {
            get
            {
                var mp = MiddlePoint;
                return new Circle2(mp, LineSegment2.CalcLenght(mp, Location));
            }
        }

        public double Left
        {
            get
            {
                return X;
            }
        }

        public double Top
        {
            get
            {
                return Y;
            }
        }


        public double Right
        {
            get
            {
                return X + Width;
            }
        }


        public double Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        #endregion

        #region Public Methods

        // TODO Tolerance handling!!

        /// <summary>
        ///  Determines if the specfied point is contained within this rectangle.     
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            return this.X <= point.X &&
            point.X < this.X + this.Width &&
            this.Y <= point.Y &&
            point.Y < this.Y + this.Height;
        }

        /// <summary>
        /// Determines if the given rect is entirely contained within this rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Contains(AARectangle rect)
        {
            return (this.X <= rect.X) &&
                   ((rect.X + rect.Width) <= (this.X + this.Width)) &&
                   (this.Y <= rect.Y) &&
                   ((rect.Y + rect.Height) <= (this.Y + this.Height));
        }


        /// <summary>
        /// Determines if this rectangle intersets with rect.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool CollidesWith(AARectangle rect)
        {
            return (rect.X < this.X + this.Width) &&
                   (this.X < (rect.X + rect.Width)) &&
                   (rect.Y < this.Y + this.Height) &&
                   (this.Y < rect.Y + rect.Height);
        }

        #endregion

        #region Operators


        /// <summary>
        /// Tests whether two rectangles have equal location and size.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AARectangle left, AARectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests whether two <see cref='System.Drawing.RectangleF'/>
        ///       objects differ in location or size.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(AARectangle left, AARectangle right)
        {
            return !(left == right);
        }

        #endregion

        #region Equality Members

        public bool Equals(AARectangle other)
        {
            return _x.Equals(other._x) && _y.Equals(other._y) && _width.Equals(other._width) && _height.Equals(other._height);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AARectangle && Equals((AARectangle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _x.GetHashCode();
                hashCode = (hashCode*397) ^ _y.GetHashCode();
                hashCode = (hashCode*397) ^ _width.GetHashCode();
                hashCode = (hashCode*397) ^ _height.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region To - Methods

        /// <summary>
        /// Turns this axis aligned rectangle in a rotatable rectangle.
        /// </summary>
        /// <returns></returns>
        public Rectangle2 ToRectangle()
        {
            return new Rectangle2(X, Y, Width, Height, Angle.Zero);
        }

        public Vertices ToVertices()
        {
            return new Vertices()
            {
                Location,
                new Vector2(Location.X + Width, Location.Y),
                new Vector2(Location.X + Width, Location.Y + Height),
                new Vector2(Location.X, Location.Y + Height)
            };
        }
        
        #endregion


        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.InvariantCulture) + ",Y=" + Y.ToString(CultureInfo.InvariantCulture) +
            ",Width=" + Width.ToString(CultureInfo.InvariantCulture) +
            ",Height=" + Height.ToString(CultureInfo.InvariantCulture) + "}";
        }
    }
}
