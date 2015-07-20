using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualLineString : Visual
    {
        private readonly LineString _lineString;

        public VisualLineString(LineString lineString)
        {
            _lineString = lineString;
        }

        public override IGeometry Geometry
        {
            get { return _lineString; }
        }

        public override void Draw(Graphics g)
        {
            var path = new GraphicsPath();
            var vertices = _lineString.ToVertices();
            if (vertices.Count() > 1)
            {
                path.AddLines(PointFUtil.ToPointFArray(vertices));
            }
            if (path.PointCount > 0)
                g.DrawPath(Pen, path);
        }

        public override Visual Clone()
        {
            var copy = new VisualLineString(_lineString.Clone() as LineString);
            copy.Prototype(this);
            return copy;
        }


    }
}
