using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    /// <summary>
    /// Represents a visual point in 2D 
    /// </summary>
    public class VisualPoint : Visual
    {
        private readonly VisualCircle _visualPoint;
        private readonly Circle2 _point;

        public VisualPoint(Vector2 point) 
            : this(new Circle2(point, 2))
        {
        }

        VisualPoint(Circle2 point)
        {
            _point = point;
            _visualPoint = new VisualCircle(_point);
        }

        public Vector2 Point
        {
            get
            {
                return _point.MiddlePoint;
            }
            set { _point.MiddlePoint = value; }
        }

        public override IGeometry Geometry
        {
            get { return _point; }
        }

        public override void Draw(Graphics g)
        {
            _visualPoint.Pen = Pen;
            _visualPoint.FillBrush = FillBrush;
            _visualPoint.Draw(g);
        }

        
        public override Visual Clone()
        {
            var copy = new VisualPoint(_point.Location);
            copy.Prototype(this);
            return copy;
        }
    }
}
