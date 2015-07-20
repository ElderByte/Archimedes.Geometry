using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry
{
    /// <summary>
    /// Represents a shape (a closed geometry)
    /// </summary>
    public interface IShape : IGeometry
    {
        /// <summary>
        /// Gets the area of this geometry shape
        /// </summary>
        double Area { get; }

        /// <summary>
        /// Turns this shape into a polygon
        /// </summary>
        /// <returns></returns>
        Polygon2 ToPolygon2();
    }
}
