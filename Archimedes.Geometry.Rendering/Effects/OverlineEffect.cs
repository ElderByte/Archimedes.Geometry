using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Archimedes.Geometry.Rendering.Primitives;

namespace Archimedes.Geometry.Rendering.Effects
{
    //Draws the Given IGeometryBase with specific Pen/Brush
    public class OverlineEffect : IEffect
    {
        private readonly Visual _geometry;

        public OverlineEffect(Visual visual, Pen effectPen = null, Brush effectBrush = null)
        {
            _geometry = visual.Clone();

            EffectPen = effectPen;
            EffectBrush = effectBrush;         
        }

        #region Public Properties

        public Pen EffectPen {
            get { return _geometry.Pen; }
            set { _geometry.Pen = value; }
        }

        public Brush EffectBrush {
            get { return _geometry.FillBrush; }
            set { _geometry.FillBrush = value; }
        }

        #endregion

        public void Draw(Graphics g) {
            _geometry.Draw(g);
        }
    }
}
