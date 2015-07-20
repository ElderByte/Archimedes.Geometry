using System.Drawing;

namespace Archimedes.Geometry.Rendering
{
    public enum ScanSideBegin
    {
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    public static class BitmapUtil
    {

        public static AARectangle BoundingBox(Bitmap oBitmap, Color backGrundColor)
        {
            var downRight = new Vector2(
                FindFirstPixelDist(oBitmap, ScanSideBegin.RIGHT, backGrundColor),
                FindFirstPixelDist(oBitmap, ScanSideBegin.BOTTOM, backGrundColor));
            var upperLeft = new Vector2(
                FindFirstPixelDist(oBitmap, ScanSideBegin.LEFT, backGrundColor),
                FindFirstPixelDist(oBitmap, ScanSideBegin.TOP, backGrundColor));

            var correction = new Vector2(0, 2);
            downRight = downRight - correction; // Correction 
            return new AARectangle(upperLeft, new SizeD((downRight.X - upperLeft.X), (downRight.Y - upperLeft.Y)));
        }


        public static int FindFirstPixelDist(Bitmap oBitmap, ScanSideBegin uScanSide, Color backGrundColor) {
            bool bColorFound = false;
            int i = 0;

            switch (uScanSide) {

            #region RIGHT

            case ScanSideBegin.RIGHT:    // ------------ Search First Pixel RIGHT 
                for (i = (oBitmap.Width - 1); i > 0; i--) {

                    for (int i2 = 1; i2 < oBitmap.Height; i2++) {
                        if (oBitmap.GetPixel(i, i2) != backGrundColor) {
                            bColorFound = true;
                            break;
                        }
                    }
                    if (bColorFound) { break; }
                }
            break;

            #endregion

            #region LEFT

            case ScanSideBegin.LEFT: // ------------ Search First Pixel LEFT
            
                for (i = 1; i < oBitmap.Width; i++) {

                    for (int i2 = 1; i2 < oBitmap.Height; i2++) {
                        if (oBitmap.GetPixel(i, i2) != backGrundColor) {
                            bColorFound = true;
                            break;
                        }
                    }
                    if (bColorFound) { break; }
                }
                //UpperLeft.X = i - 1;
            break;

            #endregion

            #region DOWN

            case ScanSideBegin.BOTTOM: // ------------ Search First Pixel Down
                for (i = (oBitmap.Height - 1); i > 0; i--) {

                    for (int i2 = 1; i2 < oBitmap.Width; i2++) {
                        if (oBitmap.GetPixel(i2, i) != backGrundColor) {
                            bColorFound = true;
                            break;
                        }
                    }
                    if (bColorFound) { break; }
                }
                //DownRight.Y = i;
            break;

            #endregion

            #region UP

            case ScanSideBegin.TOP: // ------------ Search First Pixel UP
                for (i = 1; i < oBitmap.Height; i++) {
                    for (int i2 = 1; i2 < oBitmap.Width; i2++) {
                        if (oBitmap.GetPixel(i2, i) != backGrundColor) {
                            bColorFound = true;
                            break;
                        }
                    }
                    if (bColorFound) { break; }
                }
                //UpperLeft.Y = i - 1;
            break;

            #endregion

            }

            return i;
        }
    }
}
