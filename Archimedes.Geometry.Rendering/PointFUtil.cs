using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Rendering
{
    public static class PointFUtil
    {

        public static PointF ToPointF(Vector2 vector)
        {
            return new PointF((float)vector.X, (float)vector.Y);
        }

        public static PointF[] ToPointFArray(IEnumerable<Vector2> vectors)
        {
            return ToPointFArray(vectors.ToList());
        }

        public static PointF[] ToPointFArray(List<Vector2> vectors)
        {
            var array = new PointF[vectors.Count];

            for(int i=0;i<array.Length; i++)
            {
                array[i] = ToPointF(vectors[i]);
            }
            return array;
        }
    }
}
