using System;
using System.Collections.Generic;
using System.Linq;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// Composition of several single IGeometryBase Elements to a common Interface
    /// </summary>
    public class ComplexGeometry : IGeometry
    {
        #region Fields

        readonly List<IGeometry> _geometries = new List<IGeometry>();
        Vertices _verticesCache = null;
        readonly object _verticesCacheLock = new object();

        AARectangle _boundingbox;
        Rectangle2 _boundingboxsmall;

        bool _boundingboxInvalidated = true;
        bool _boundingboxsmallInvalidated = true;

        #endregion

        #region Constructor

        public ComplexGeometry() { }

        #endregion

        #region Geometry Access Methods

        public void AddGeometry(IGeometry geometry) {
            _geometries.Add(geometry);
            Invalidate();
        }

        public void AddGeometries(IEnumerable<IGeometry> geometries) {
            _geometries.AddRange(geometries);
            Invalidate();
        }

        public void RemoveGeometry(IGeometry geometry) {
            _geometries.Remove(geometry);
            Invalidate();
        }

        public int Count() {
            return _geometries.Count();
        }

        public IGeometry this[int index]{
            get {
                return _geometries[index];
            }
            set {
                _geometries[index] = value;
                Invalidate();
            }
        }

        #endregion

        #region Public Properties

        public Vector2 FirstPoint {
            get { return ToPath().FirstPoint; }
        }

        public Vector2 LastPoint {
            get { return ToPath().LastPoint; }
        }


        private void Invalidate() {
            lock (_verticesCacheLock)
            {
                _verticesCache = null;
            }
            _boundingboxInvalidated = true;
            _boundingboxsmallInvalidated = true;
        }

        public Vector2 Location {
            get { return MiddlePoint; }
            set { MiddlePoint = value; }
        }

        public Vector2 MiddlePoint {
            get {

                double mpointX = 0;
                double mpointY = 0;

                var middlepoints = (from g in _geometries
                                   select g.MiddlePoint).ToList();

                foreach (var pnt in middlepoints){
                    mpointX += pnt.X;
                    mpointY += pnt.Y;
                }

               return new Vector2(
                   mpointX / middlepoints.Count,
                   mpointY / middlepoints.Count);
            }
            set {
                var mov = new Vector2(MiddlePoint, value);
                this.Translate(mov);
            }
        }

        #endregion

        #region Public Methods

        public void Translate(Vector2 mov) {
            foreach (var g in _geometries)
                g.Translate(mov);
            Invalidate();
        }

        public void Scale(double fact) {
            foreach (var g in _geometries)
                g.Scale(fact);
            Invalidate();
        }


        /// <summary>
        /// Gets a deep copy of this Object
        /// </summary>
        /// <returns></returns>
        public IGeometry Clone() {
            var nuv = new ComplexGeometry();
            foreach (var g in _geometries)
                nuv.AddGeometry(g.Clone());
            return nuv;
        }

        public void Prototype(IGeometry iprototype) {

            var prototype = iprototype as ComplexGeometry;
            if (prototype == null)
                throw new NotSupportedException();

            _geometries.Clear();
            _geometries.AddRange(prototype.GetGeometries());
        }

        public bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            foreach (var g in _geometries) {
                if (g.Contains(point, tolerance)) {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Vector2 point, ref IGeometry subGeometry, double tolerance = GeometrySettings.DEFAULT_TOLERANCE) {
            subGeometry = null;
            foreach (var g in _geometries) {
                if (g.Contains(point, tolerance))
                {
                    subGeometry = g;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Bounding Boxes

        public AARectangle BoundingBox {
            get {
                if (_boundingboxInvalidated) {
                    _boundingbox = ToVertices().BoundingBox;
                    _boundingboxInvalidated = false;
                }
                return _boundingbox;
            }
        }

        public Rectangle2 SmallestBoundingBox {
            get {
                if (_boundingboxsmallInvalidated) {
                    _boundingboxsmall = ToPolygon2().FindSmallestBoundingBox();
                    _boundingboxsmallInvalidated = false;
                }
                return _boundingboxsmall;
            }
        }

        public Rectangle2 SmallestWidthBoundingBox {
            get {
               return ToPolygon2().FindSmallestWidthBoundingBox();
            }
        }
        

        public Circle2 BoundingCircle {
            get { return this.ToPolygon2().BoundingCircle; }
        }

#endregion

        #region Intersection Methods

        public IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
           var intercepts = new List<Vector2>();
           foreach (var g in _geometries) {
               intercepts.AddRange(g.Intersect(other, tolerance));
           }
           return intercepts;
        }

        public bool HasCollision(IGeometry geometry, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            foreach (var g in _geometries) {
                if (g.HasCollision(geometry, tolerance))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region To -> Transformer Methods

        /// <summary>
        /// Returns vertices of this geometry
        /// </summary>
        /// <returns></returns>
        public Vertices ToVertices() {
            lock (_verticesCacheLock) {
                if (_verticesCache == null)
                {
                    var vertices = ToPath().ToVertices().Distinct();
                    _verticesCache = new Vertices(vertices);
                }
                return new Vertices(_verticesCache);
            }
        }

        public IEnumerable<IGeometry> GetGeometries() {
            return new List<IGeometry>(_geometries);
        }

        public Polygon2 ToPolygon2() {
            return new Polygon2(ToVertices());
        }

        /// <summary>
        /// Creates a docked path from this geometries.
        /// </summary>
        /// <returns></returns>
        public LineString ToPath() {
            var path = new LineString();
            foreach (var g in _geometries)
                path.DockGeometry(g);
            return path;
        }

        #endregion

    }
}
