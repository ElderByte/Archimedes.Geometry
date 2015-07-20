using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Algorithms
{
    /// <summary>
    /// Base Class for all polygon-based BoundingBox finder Algorythms
    /// This Class is abstract
    /// </summary>
    public interface IPolygonBoundingBoxAlgorythm
    {


      /*
        public virtual void SetPolygon(Polygon2 poly) {
            _polygon = poly.Clone() as Polygon2;
            _vertices = _polygon.ToVertices().Distinct().ToArray();
        }*/

        /// <summary>
        /// Process the set Polygon with the underlying algorythm and find the Boundingbox.
        /// </summary>
        /// <returns>Returns the Boundingbox as Array of 4 Points</returns>
        Vector2[] FindBounds(Polygon2 polygon);
    }
}
