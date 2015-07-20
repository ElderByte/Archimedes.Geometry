using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Rendering
{
    public static class RectangleFUtil
    {

        public static RectangleF ToRectangleF(AARectangle aarect)
        {
            return new RectangleF((float)aarect.X, (float)aarect.Y, (float)aarect.Width, (float)aarect.Height);
        }

    }
}
