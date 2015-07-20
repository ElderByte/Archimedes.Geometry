using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualPolygon : Visual
    {
        private readonly Polygon2 _polygon;


        public VisualPolygon(Polygon2 polygon)
        {
            _polygon = polygon;
        }


        public override IGeometry Geometry
        {
            get { return _polygon; }
        }

        public Polygon2 Polygon
        {
            get { return _polygon; }
        }

        public override void Draw(Graphics g)
        {
            try
            {
                var path = new GraphicsPath();
                path.AddPolygon(PointFUtil.ToPointFArray(_polygon.ToVertices()));
                if (this.FillBrush != null)
                    g.FillPath(this.FillBrush, path);
                g.DrawPath(Pen, path);
            }
            catch (Exception)
            {
                //ignore
            }
        }


        public override Visual Clone()
        {
            var copy = new VisualPolygon(_polygon.Clone() as Polygon2);
            copy.Prototype(this);
            return copy;
        }
    }
}
