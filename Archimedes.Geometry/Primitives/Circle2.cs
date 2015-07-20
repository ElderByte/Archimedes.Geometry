/*******************************************
 * 
 *  Written by Pascal Büttiker (c)
 *  April 2010
 * 
 * *****************************************
 * *****************************************/


using System;
using System.Collections.Generic;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// Represents a circle shape in 2d Space
    /// </summary>
    public partial class Circle2 : IShape
    {
        #region Fields

        Vector2 _middlePoint;
        double _radius;

        bool _verticesInValidated = true;
        Vertices _vertices = new Vertices();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty circle
        /// </summary>
        public Circle2() { }

        /// <summary>
        /// Creates a new circle at the given location with the given radius
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="uRadius"></param>
        public Circle2(double x, double y, double uRadius)
        {
            _middlePoint = new Vector2(x, y);
            _radius = uRadius;
        }



        public Circle2(Vector2 uMiddlePoint, double uRadius)
        {
            _middlePoint = uMiddlePoint;
            _radius = uRadius;
        }

        public Circle2(Circle2 prototype) {
            Prototype(prototype);
        }


        #endregion

        #region Exportet Properties

        public double Radius
        {
            get { return _radius; }
            set { 
                _radius = value;
                _verticesInValidated = true;
            }
        }


        #endregion

        #region Exp Methods

        /// <summary>
        /// Returns a point on this circle at the given angle offset.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2 GetPoint(Angle angle){
            var circlePoint = new Vector2(
                Math.Cos(angle.Radians) * this.Radius,
                Math.Sin(angle.Radians) * this.Radius);

            circlePoint += this.Location;
            return circlePoint;
        }

        #endregion

        #region Geometry Base

        public void Scale(double factor)
        {
            this.Location = Location.Scale(factor);
            Radius *= factor;
        }

        public void Translate(Vector2 mov) {
            this.Location += mov;
        }

        public Vector2 Location {
            get { return this.MiddlePoint; }
            set { this.MiddlePoint = value; }
        }

        public Vector2 MiddlePoint {
            get { return _middlePoint; }
            set { _middlePoint = value; }
        }

        public IGeometry Clone() {
            return new Circle2(this);
        }

        public void Prototype(IGeometry iprototype) {
            this.Location = iprototype.Location;
            this.Radius = (iprototype as Circle2).Radius;
        }


        #endregion

        #region GeomertryBase Collision

        public virtual bool HasCollision(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (other is LineSegment2)
            {
                return this.InterceptLineWith(other as LineSegment2, tolerance);
            }
            else if (other is Circle2)
            {
                return this.InterceptWithCircle(other as Circle2, tolerance);
            }
            else if (other is Polygon2)
            {
                return InterceptPolygon(((Polygon2) other), tolerance).Count != 0;
            }
            else if (other is Rectangle2)
            {
                return InterceptPolygon(((Rectangle2) other).ToPolygon2(), tolerance).Count != 0;
            }
            else if (other is Arc)
            {
                // inverse call:
                return other.HasCollision(this, tolerance);
            }
            else
                return this.ToPolygon2().HasCollision(other, tolerance);
        }

        public virtual IEnumerable<Vector2> Intersect(IGeometry other,
            double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            if (other is LineSegment2)
            {
                return this.InterceptLine(other as LineSegment2, tolerance);
            }
            else if (other is Circle2)
            {
                return this.InterceptCircle(other as Circle2, tolerance);
            }
            else if (other is Polygon2)
            {
                return InterceptPolygon(other as Polygon2, tolerance);
            }
            else if (other is Rectangle2)
            {
                return InterceptPolygon((other as Rectangle2).ToPolygon2(), tolerance);
            }
            else if (other is Arc)
            {
                // inverse call:
                return other.Intersect(this, tolerance);
            }
            else
                return this.ToPolygon2().Intersect(other, tolerance);
        }

        #endregion

        #region To-Methods

        public virtual Vertices ToVertices() {
            if (_verticesInValidated) {
                _vertices.Clear();
                _vertices.AddRange(Flatten());
            }
            return new Vertices(_vertices);
        }

        public Polygon2 ToPolygon2()
        {
            return new Polygon2(this.ToVertices());
        }

        private IEnumerable<Vector2> Flatten(int resolution = 50)
        {
            for (int i = 0; i < resolution; i++)
            {
                double step = 360.0 / resolution * i;
                yield return GetPoint(Angle.FromDegrees(step));
            }
            yield return GetPoint(Angle.FromDegrees(360));
        } 

        #endregion

        #region IShape

        public double Area {
            get { return Math.Pow(Radius, 2) * Math.PI; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Radius: {0}, MiddlePoint: {1}, Area: {2}", Radius, MiddlePoint, Area);
        }
    }
}
