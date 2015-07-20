using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualComposite : Visual
    {
        private readonly List<Visual> _children = new List<Visual>();
        private readonly ComplexGeometry _geometry = new ComplexGeometry(); 
        
        public VisualComposite()
        {
            
        }


        public void Add(Visual child)
        {
            _children.Add(child);
            _geometry.AddGeometry(child.Geometry);
        }


        public override IGeometry Geometry
        {
            get { return _geometry; }
        }

        public override void Draw(Graphics g)
        {
            foreach (var child in _children)
            {
                child.Draw(g);
            }
        }

        public override Visual Clone()
        {
            var clone = new VisualComposite();
            foreach (var child in _children)
            {
                clone.Add(child.Clone());
            }
            return clone;
        }
    }
}
