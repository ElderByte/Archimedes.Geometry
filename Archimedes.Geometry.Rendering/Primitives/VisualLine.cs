using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualLine : Visual
    {
        private readonly LineSegment2 _line;

        public VisualLine(LineSegment2 line)
        {
            _line = line;
        }

        public override IGeometry Geometry
        {
            get { return _line; }
        }

        public LineSegment2 Line
        {
            get { return _line; }
        }

        public override void Draw(Graphics g)
        {
            if (this.Pen != null && !_line.Start.Equals(_line.End))
                g.DrawLine(this.Pen, PointFUtil.ToPointF(_line.Start), PointFUtil.ToPointF(_line.End));
        }

        public override Visual Clone()
        {
            var copy = new VisualLine(_line.Clone() as LineSegment2);
            copy.Prototype(this);
            return copy;
        }
    }
}
