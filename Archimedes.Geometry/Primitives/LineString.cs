using System;
using System.Collections.Generic;
using System.Linq;
using Archimedes.Geometry.Algorithms;
using Vertex = Archimedes.Geometry.Vector2;

namespace Archimedes.Geometry.Primitives
{
    /// <summary>
    /// A line string of connected points.
    /// // TODO all intersection methods missing
    /// </summary>
    public class LineString : IGeometry
    {
        #region Fields

        private readonly Vertices _vertices = new Vertices();

        #endregion

        #region Construcors

        /// <summary>
        /// Creates an empty line string
        /// </summary>
        public LineString() { }

        /// <summary>
        /// Creates a new line string with same values as the prototype
        /// </summary>
        /// <param name="prototype"></param>
        public LineString(LineString prototype) 
            : this() {
            this.Prototype(prototype);
        }

        /// <summary>
        /// Creates a new line string with the given vertices
        /// </summary>
        /// <param name="vertices"></param>
        public LineString(IEnumerable<Vertex> vertices)
            : this()
        {
            Append(vertices);
        }

        public LineString(IGeometry geometry)
            : this() {
                AddGeometry(geometry);
        }
        public LineString(IEnumerable<IGeometry> geometries)
            : this() {
            foreach (var g in geometries)
                AddGeometry(g);
        }


        #endregion

        #region Data Access

        /// <summary>
        /// Adds the given verices this this line string
        /// </summary>
        /// <param name="other"></param>
        public void Append(IEnumerable<Vector2> other)
        {
            _vertices.AddRange(other);
        }

        /// <summary>
        /// Adds  the <paramref name="geometry"/> to the path.
        /// </summary>
        /// <param name="geometry">Geomtry to add</param>
        public void AddGeometry(IGeometry geometry) {
            Append(geometry.ToVertices());
        }


        /// <summary>
        /// Tries to dock the <paramref name="geometry"/> to this path. 
        /// </summary>
        /// <param name="geometry">Geometry to dock on this path</param>
        public void DockGeometry(IGeometry geometry) {
            DockVertices(geometry.ToVertices());
        }


        public void DockVertices(IEnumerable<Vertex> vertices) {
            // does vertex order matther?
            if (!_vertices.Any()) {
                Append(vertices);
                return;
            }

            // sort the vertices to make connection possible
            var connector = new PathConnector(this.ToVertices(), vertices);
            Clear();
            Append(connector.ConnectPaths());
        }

        public void Clear() {
            _vertices.Clear();
        }

        #endregion

        public Vector2 LastPoint {
            get { return _vertices.LastOrDefault(); }
        }

        public Vector2 FirstPoint {
            get {
                return _vertices.First();
            }
        }

        public virtual void Dispose() {
        }

        #region IGeometryBase

        public Vector2 Location {
            get { return MiddlePoint; }
            set { MiddlePoint = value; }
        }

        public Vector2 MiddlePoint {
            get {
                return BoundingBox.MiddlePoint;
            }
            set {
                var move = new Vector2(this.MiddlePoint, value);
                this.Translate(move);
            }
        }

        public IGeometry Clone() {
            return new LineString(this);
        }

        public void Prototype(IGeometry prototype) {
            Clear();
            Append(prototype.ToVertices());
        }

        public void Translate(Vector2 mov) {
            _vertices.Translate(mov);
        }

        public void Scale(double fact)
        {
            _vertices.Scale(fact);
        }

        public Vertices ToVertices() {
            return new Vertices(_vertices);
        }

        public AARectangle BoundingBox {
            get { return _vertices.BoundingBox; }
        }


        public Circle2 BoundingCircle {
            get { return BoundingBox.BoundingCircle; }
        }

        public bool HasCollision(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vector2 pnt, double tolerance = GeometrySettings.DEFAULT_TOLERANCE)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
