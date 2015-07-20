using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualCircle : Visual
    {
        private readonly Circle2 _circle;


        public VisualCircle(Circle2 circle)
        {
            _circle = circle;
        }


        public override IGeometry Geometry
        {
            get { return _circle; }
        }

        public override void Draw(Graphics g)
        {
            try
            {
                var bounds = RectangleFUtil.ToRectangleF(_circle.BoundingBox);

                if (this.FillBrush != null)
                    g.FillEllipse(this.FillBrush, bounds);
                if (this.Pen != null)
                    g.DrawArc(this.Pen, bounds, 0, 360);
            }
            catch (Exception)
            {
                //ignore
            }
        }


        public override Visual Clone()
        {
            var copy = new VisualCircle(_circle.Clone() as Circle2);
            copy.Prototype(this);
            return copy;
        }

    }
}
