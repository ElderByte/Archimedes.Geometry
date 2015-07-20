using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;
using Archimedes.Geometry.Units;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualArc : Visual
    {
        private readonly Arc _arc;

        public VisualArc(Arc arc)
        {
            _arc = arc;
        }

        public override IGeometry Geometry
        {
            get { return _arc; }
        }

        public Arc Arc {
            get { return _arc; }
        }

        public override void Draw(Graphics g)
        {
            if (this.Pen != null)
                g.DrawArc(
                    this.Pen,
                    CalcDrawingRect(),
                    (float)(_arc.Angle2X.Degrees - 90),
                    (float)_arc.Angle.Degrees);
        }

        public override Visual Clone()
        {
            var copy = new VisualArc(_arc.Clone() as Arc);
            copy.Prototype(this);
            return copy;
        }


        private RectangleF CalcDrawingRect()
        {
            var middlePoint = _arc.MiddlePoint;
            return new RectangleF(
                (float)(middlePoint.X - _arc.Radius),
                (float)(middlePoint.Y - _arc.Radius),
                2 * (float)_arc.Radius,
                2 * (float)_arc.Radius);
        }

    }
}
