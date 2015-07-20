using System;

namespace Archimedes.Geometry.Algorithms
{
    /// <summary>
    /// Algorithm which finds the boundingbox with the smallest width.
    /// </summary>
    public class PolygonSmallestWidthBoundingBoxAlgorythm : PolygonSmallestBoundingBoxAlgorythm
    {
        SizeD? _bestRectDimension;

        protected override bool IsCurrentRectangleTheBest(Vector2[] currentRectangle)
        {
            bool isBetter = false;

            var vx0 = currentRectangle[0].X - currentRectangle[1].X;
            var vy0 = currentRectangle[0].Y - currentRectangle[1].Y;
            var len0 = Math.Sqrt(vx0 * vx0 + vy0 * vy0);

            var vx1 = currentRectangle[1].X - currentRectangle[2].X;
            var vy1 = currentRectangle[1].Y - currentRectangle[2].Y;
            var len1 = Math.Sqrt(vx1 * vx1 + vy1 * vy1);

            var thisRectDim = CreateArea(len0, len1);

            if (_bestRectDimension.HasValue) {
                if (_bestRectDimension.Value.Width > thisRectDim.Width) {
                    _bestRectDimension = thisRectDim;
                    isBetter = true;
                }
            } else {
                _bestRectDimension = thisRectDim;
                isBetter = true;
            }
            return isBetter;
        }

        private static SizeD CreateArea(double n1, double n2)
        {
            return (n1 >= n2) ?  new SizeD(n2, n1) : new SizeD(n1, n2);
        }


    }
}
