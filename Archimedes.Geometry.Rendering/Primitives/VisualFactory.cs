using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    /// <summary>
    /// This factory helps to construct visuals from geometry objects.
    /// </summary>
    public static class VisualFactory
    {
        #region Static Creator methods

        public static VisualPoint Create(Vector2 point, Brush brush)
        {
            return new VisualPoint(point)
            {
                FillBrush = brush
            };
        }

        public static VisualLine Create(LineSegment2 line, Pen pen = null)
        {
            return new VisualLine(line)
            {
                Pen = pen
            };
        }

        public static VisualArc Create(Arc arc, Pen pen = null)
        {
            return new VisualArc(arc)
            {
                Pen = pen
            };
        }

        public static VisualCircle Create(Circle2 circle, Pen pen = null, Brush brush = null)
        {
            return new VisualCircle(circle)
            {
                Pen = pen,
                FillBrush = brush
            };
        }

        public static VisualPolygon Create(Polygon2 poly, Pen pen = null, Brush brush = null)
        {
            return new VisualPolygon(poly)
            {
                Pen = pen,
                FillBrush = brush
            };
        }

        public static VisualRectangle Create(Rectangle2 rect, Pen pen = null, Brush brush = null)
        {
            return new VisualRectangle(rect)
            {
                Pen = pen,
                FillBrush = brush
            };
        }

        public static VisualAARectangle Create(RectangleAA2 rect, Pen pen = null, Brush brush = null)
        {
            return new VisualAARectangle(rect)
            {
                Pen = pen,
                FillBrush = brush
            };
        }

        public static VisualLineString Create(LineString lineString, Pen pen = null)
        {
            return new VisualLineString(lineString)
            {
                Pen = pen
            };
        }

        public static VisualText Create(Vector2 location, string text, Pen pen = null, Brush background = null, Font font = null)
        {
            return new VisualText(location, text, pen, font)
            {
                FillBrush = background
            };
        }

        #endregion


        public static Visual CreateGeneric(IGeometry geometry, Pen pen = null, Brush brush = null)
        {
            if (geometry is LineSegment2)
            {
                return Create(geometry as LineSegment2, pen);
            }
            if (geometry is Arc)
            {
                return Create(geometry as Arc, pen);
            }
            if (geometry is Circle2)
            {
                return Create(geometry as Circle2, pen, brush);
            }
            if (geometry is Rectangle2)
            {
                return Create(geometry as Rectangle2, pen, brush);
            }
            if (geometry is RectangleAA2)
            {
                return Create(geometry as RectangleAA2, pen, brush);
            }
            if (geometry is Polygon2)
            {
                return Create(geometry as Polygon2, pen, brush);
            }
            if (geometry is LineString)
            {
                return Create(geometry as LineString, pen);
            }

            throw new NotSupportedException("The given geometry " + geometry.GetType().Name + " is not supported yet by the visual builder!");
        }

    }
}
