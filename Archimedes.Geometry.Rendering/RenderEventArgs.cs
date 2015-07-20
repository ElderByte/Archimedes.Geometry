using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Archimedes.Geometry.Rendering
{
    public class RenderEventArgs : EventArgs
    {
        readonly Graphics _g;

        public RenderEventArgs(Graphics g) {
            _g = g;
        }

        /// <summary>
        /// Graphics Context
        /// </summary>
        public Graphics Graphics {
            get { return _g; }
        }
    }
}
