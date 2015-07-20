using System.Collections.Generic;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry
{
    /// <summary>
    /// The base of every Geometry Object, which can be anything in 2D space.
    /// For example a line, arc or any shape / polygon
    /// </summary>
    public interface IGeometry
    {
        /// <summary>
        /// The location of this geometry? 
        /// </summary>
        Vector2 Location { get; set; }

        /// <summary>
        /// Gets / Sets the middlepoint of this geometry
        /// </summary>
        Vector2 MiddlePoint { get; set; }
        
        /// <summary>
        /// Creates a copy of this geometry
        /// </summary>
        /// <returns></returns>
        IGeometry Clone();

        /// <summary>
        /// Copy all values from the given prototype ot this instance
        /// </summary>
        /// <param name="prototype"></param>
        void Prototype(IGeometry prototype);

        /// <summary>
        /// Translates (moves) this geometry by the given vector
        /// </summary>
        /// <param name="mov">The translation vector</param>
        void Translate(Vector2 mov);

        /// <summary>
        /// Scales this geometry by the given factor
        /// </summary>
        /// <param name="factor">The scale factor</param>
        void Scale(double factor);

        /// <summary>
        /// Turns this geometry into a set of vertices
        /// </summary>
        /// <returns></returns>
        Vertices ToVertices();


        /// <summary>
        /// Returns an axis aligned bounding box (AABB) which fully encloses the IGeometryBase object.
        /// </summary>
        AARectangle BoundingBox { get; } // TODO Dont use Drawing.Rectange but a custom AABB

        /// <summary>
        /// Returns a Circle which fully encloses the IGeometryBase object
        /// </summary>
        Circle2 BoundingCircle { get; }

        /// <summary>
        /// Checks if this geometry collides with the given other one.
        /// </summary>
        /// <param name="geometryObject"></param>
        /// <param name="tolerance"></param>
        /// <returns>true if the objects collide</returns>
        bool HasCollision(IGeometry geometryObject, double tolerance = GeometrySettings.DEFAULT_TOLERANCE);

        /// <summary>
        /// Checks Intersection and returns all Points at intersection joints
        /// </summary>
        /// <param name="other"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        IEnumerable<Vector2> Intersect(IGeometry other, double tolerance = GeometrySettings.DEFAULT_TOLERANCE);

        /// <summary>
        /// Checks if a Point is contained in the geometry
        /// </summary>
        /// <param name="point"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        bool Contains(Vector2 point, double tolerance = GeometrySettings.DEFAULT_TOLERANCE);

      
    }
}
