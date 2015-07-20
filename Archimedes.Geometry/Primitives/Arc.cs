using System;
using System.Collections.Generic;
using Archimedes.Geometry.Units;

/*******************************************
 * 
 *  Written by Pascal Büttiker (c)
 *  2010
 * 
 * *****************************************
 * *****************************************/


namespace Archimedes.Geometry.Primitives
{

    /// <summary>
    /// Represents an Arc in 2D Space
    /// </summary>
    public partial class Arc : IGeometry
    {
        #region Private Data

        double? _radius = null;
        Angle? _angle = null;
        double? _bowlen = null;
        Direction _direction = Direction.LEFT;
        Vector2 _startPoint;
        Vector2 _base;

        #endregion

        #region Contructors

        public Arc() { }

        /// <summary>
        /// Creates a new Arc with the same values as the given prototype
        /// </summary>
        /// <param name="prototype"></param>
        public Arc(Arc prototype)
        {
            Prototype(prototype);
        }


        /// <summary>
        /// Creates a new Arc which goes through the given 3 points.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="inter"></param>
        /// <param name="end"></param>
        public Arc(Vector2 start, Vector2 inter, Vector2 end)
        {
           Prototype(ArcBuilder.FromDescriptorPoints(start, inter, end));
        }

        /// <summary>
        /// Creates a new Arc
        /// </summary>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="angle">The arc angle</param>
        /// <param name="baseVector">A vector of the arc start</param>
        public Arc(double? radius, Angle? angle, Vector2 baseVector)
        {
            _angle = angle;
            _radius = radius;
            _base = baseVector;
            _bowlen = null;
        }

        /// <summary>
        /// Creates a new Arc
        /// </summary>
        /// <param name="radius">The radius of the arc</param>
        /// <param name="bowLen">The lenght of the arc bow</param>
        /// <param name="baseVector">The base vector of the arc start</param>
        public Arc(double? radius,  double? bowLen, Vector2 baseVector)
        {
            _angle = null;
            _radius = radius;
            _bowlen = bowLen;
            _base = baseVector;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a point on this arc, with a delta angle from the arc start point.
        /// </summary>
        /// <param name="deltaAngle"></param>
        /// <returns></returns>
        public Vector2 GetPointOnArc(Angle deltaAngle)
        {
            var helperArc = new Arc(Radius, deltaAngle, _base);
            helperArc.Direction = this.Direction;
            helperArc.Location = this.Location;

            var relAngle = CalcRelAngle(
                helperArc.Angle,
                _base.AngleSignedTo(Vector2.UnitX, true),
                helperArc.Direction);

            var pointOnArc = CalcEndpointDelta2M(helperArc.Radius, relAngle);

            var helperMP = helperArc.MiddlePoint;
            pointOnArc = new Vector2(pointOnArc.X + helperMP.X, pointOnArc.Y + helperMP.Y);

            return pointOnArc;
        }


        public void Scale(double factor)
        {
            this.Location = this.Location.Scale(factor);

            if (_radius.HasValue)
                _radius *= factor;

            if (_bowlen.HasValue)
                _bowlen *= factor;
        }

        #endregion

        #region Public Propertys


        public Vector2 BaseVector
        {
            get { return _base; }
            set { _base = value; }
        }




        public Direction Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
            }
        }

        public double Radius
        {
            set
            {
                _radius = value;
                if (_angle.HasValue && _bowlen.HasValue)
                    _bowlen = null;
            }
            get
            {
                if (_radius == null)
                { // radius not set!
                    // possible to calc radius?
                    if (_bowlen != null && _angle != null && _angle != Angle.Zero)
                    {
                        return (180 * _bowlen.Value) / (Math.PI * _angle.Value.Degrees);
                    }
                    else
                    {
                        return 0f;
                    }
                }
                else
                {
                    return _radius.Value;
                }

            }
        }

        /// <summary>
        /// The Arc Rotation
        /// </summary>
        public Angle Angle
        {
            set
            {
                _angle = value;
                if (_radius.HasValue && _bowlen.HasValue)
                    _bowlen = null;
            }
            get
            {
                if (_angle.HasValue)
                {
                    return _angle.Value;
                }

                // angle not set!
                // possible to calc angle?
                if (_bowlen.HasValue && _radius.HasValue && _radius != 0)
                {
                    return Angle.FromDegrees(180 * _bowlen.Value) / (Math.PI * _radius.Value);
                }

                return Angle.Zero;
            }
        }


        /// <summary>
        /// Lenght of the Arc-Bow-Line
        /// </summary>
        public double BowLen
        {
            set
            {
                _bowlen = value;
                if (_radius.HasValue && _angle.HasValue)
                    _radius = null;
            }
            get
            {
                if (_bowlen == null)
                {

                    // possible to calc bowlen?
                    if (_radius != null && _angle != null)
                    {
                        return (_radius.Value * Math.PI * (_angle.Value.Degrees / 180));
                    }
                    else
                    {
                        return 0f;
                    }
                }
                else
                {
                    return _bowlen.Value;
                }
            }
        }

        #endregion

        #region Read only properties


        /// <summary>
        /// Effective Start Rotation to draw
        /// </summary>
        public Angle Angle2X
        {
            get
            {
                var angle = _base.AngleSignedTo(Vector2.UnitX, true);

                // Correct angle if we have opposite direction
                if (Direction == Direction.RIGHT)
                {
                    angle -= (this.Angle - Angle.FromDegrees(180));
                }
                return angle;
            }
        }

        /// <summary>
        /// Returns a
        /// </summary>
        public Vector2 EndVector
        {
            get
            {
                return Vector2.FromAngleAndLenght(this.Angle, 1);
            }
        }

        /// <summary> 
        /// Get the Arc's Endpoint
        /// </summary>
        public Vector2 EndPoint
        {
            get
            {
                return GetPointOnArc(this.Angle);
            }
        }

        #endregion

        #region Specail Transformation

        /// <summary>
        /// Split the Arc at the given Point
        /// </summary>
        /// <param name="splitPoint">Point to split</param>
        /// <returns></returns>
        public IEnumerable<Arc> Split(Vector2 splitPoint)
        {
            var vstart = new Vector2(this.MiddlePoint, this.Location);
            var vsplit = new Vector2(this.MiddlePoint, splitPoint);
            return Split(vstart.GetAngleTo(vsplit));
        }

        /// <summary>
        /// Split a given Rotation-Amount from this Arc
        /// </summary>
        /// <param name="splitAngle"></param>
        /// <returns></returns>
        public IEnumerable<Arc> Split(Angle splitAngle)
        {
            var arcs = new List<Arc>();

            splitAngle = Angle.Abs(splitAngle);
            if ((splitAngle > this.Angle))
            {
                arcs.Add(this); // can't split...
            }
            else
            { /* split the arc */

                //segment uno
                var split = new Arc(this.Radius, splitAngle, _base);
                split.Location = this.Location;
                split.Direction = this.Direction;
                //split.AngleDiff = this.AngleDiff;
                arcs.Add(split);

                //segment due
                Vector2 newBase;
                if (this.Direction == Direction.LEFT)
                {
                    newBase = _base.GetRotated(splitAngle);
                }
                else
                {
                    newBase = _base.GetRotated(-splitAngle);
                }
                split = new Arc(this.Radius, this.Angle - splitAngle, newBase);
                split.Location = this.GetPointOnArc(splitAngle);
                split.Direction = this.Direction;
                //split.AngleDiff = this.AngleDiff;
                arcs.Add(split);
            }
            return arcs;
        }
        #endregion

        #region To Methods

        public Vertices ToVertices()
        {
            Vertices vertices = new Vertices();
            vertices.AddRange(Flatten());
            return vertices;
        }


        private IEnumerable<Vector2> Flatten(int resolution = 50)
        {
            for (int i = 0; i < resolution; i++)
            {
                double step = Angle.Degrees / resolution * i;
                yield return GetPointOnArc(Angle.FromDegrees(step));
            }
            yield return GetPointOnArc(Angle);
        } 

        public Circle2 ToCircle()
        {
            var c = new Circle2(this.MiddlePoint, this.Radius);
            return c;
        }

        #endregion

        #region Geomerty Base

        /// <summary>
        /// Gets or sets the middlepoint of this arc
        /// </summary>
        public Vector2 MiddlePoint
        {
            get
            {
                // Calc vector which points to the arc middlepoint
                var orthBaseVector = _base.GetOrthogonalVector(Direction).WithLength(Radius);

                return new Vector2((Location.X + orthBaseVector.X), (Location.Y + orthBaseVector.Y));
            }
            set
            {
                var delta = new Vector2(MiddlePoint, value);
                Translate(delta);
            }
        }

        /// <summary>
        /// Translate this arc by the given vector
        /// </summary>
        /// <param name="mov"></param>
        public void Translate(Vector2 mov)
        {
            this.Location += mov;
        }

        /// <summary> 
        /// Arc StartPoint
        /// </summary>
        public Vector2 Location
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
            }
        }

        public IGeometry Clone()
        {
            return new Arc(this);
        }

        public void Prototype(IGeometry iprototype)
        {

            var prototype = (iprototype as Arc);
            if (prototype == null)
                throw new InvalidOperationException();

            _angle = prototype.Angle;
            _radius = prototype.Radius;
            _bowlen = prototype.BowLen;
            _base = prototype.BaseVector;

            Location = prototype.Location;
            Direction = prototype.Direction;
        }



        public Circle2 BoundingCircle
        {
            get { return this.ToCircle(); }
        }

        public AARectangle BoundingBox
        {
            get
            {
                AARectangle box = AARectangle.Empty;
                try
                {
                    box = new Polygon2(ToVertices()).BoundingBox;
                }
                catch (Exception)
                {
                    //igonre - return void boundingbox
                }
                return box;
            }
        }

        public Rectangle2 BoundingBoxSmallest
        {
            get
            {
                Rectangle2 box;
                try
                {
                    box = new Polygon2(ToVertices()).FindSmallestBoundingBox();
                }
                catch (Exception)
                {
                    box = Rectangle2.Empty;
                }
                return box;
            }
        }

        #endregion

        #region Private methods


        /// <summary> 
        /// Calc the delta from the endpoint to middle
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        private static Vector2 CalcEndpointDelta2M(double radius, Angle rel)
        {
            return new Vector2(
                radius * Math.Cos(rel.Radians),
                radius * Math.Sin(rel.Radians));
        }

        /// <summary> 
        /// Calc Relative angle
        /// </summary>
        /// <param name="bowAngle">Bogenwinkel</param>
        /// <param name="baseAngle">Winkel des Base Vector</param>
        /// <param name="direction">Links oder Rechts</param>
        /// <returns>Relativer Winkel</returns>
        private static Angle CalcRelAngle(Angle bowAngle, Angle baseAngle, Direction direction)
        {
            bowAngle = bowAngle.Normalize();

            var relAngle = bowAngle + baseAngle;

            if (direction == Direction.LEFT)
            {
                relAngle += Angle.FromDegrees(270);
            }
            else
            {
                relAngle += Angle.FromDegrees(90) - 2 * (bowAngle - Angle.FromDegrees(180));
            }

            return relAngle.Normalize();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("Direction: {0}, Angle: {1}, BowLen: {2}, BaseVector: {3}, Radius: {4}", Direction, Angle, BowLen, BaseVector, Radius);
        }

        #endregion

    }
}
