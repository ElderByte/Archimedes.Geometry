using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Archimedes.Geometry.Primitives;
using log4net;

namespace Archimedes.Geometry.Rendering.Primitives
{
    /// <summary>
    /// Represents a Text in 2D space
    /// </summary>
    public class VisualText : Visual
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RectangleAA2 _rectangle = RectangleAA2.Empty;

        private string _text;
        private Font _font;
        private StringAlignment _textHorizontalAlign;

        private SizeD? _textSizeCache = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new visual text at the given location
        /// </summary>
        /// <param name="location"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public VisualText(Vector2 location, string text, Pen pen = null, Font font = null, Brush background = null)
        {
            Location = location;
            _text = text;
            Pen = pen ?? Pens.Black;
            _font = font ?? SystemFonts.DefaultFont;
            FillBrush = background;
        }

        #endregion

        #region Properties

        public Vector2 Location
        {
            get { return _rectangle.Location; }
            set
            {
                _rectangle.Location = value;
            }
        }

        
        public Vector2 MiddlePoint
        {
            get
            {
                return Geometry.MiddlePoint;
            }
            [DebuggerStepThrough]
            set
            {
                var geo = Geometry;
                geo.MiddlePoint = value;
            }
        }



        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                InvalidateTextSize();
            }
        }

        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                InvalidateTextSize();
            }
        }

        /// <summary>
        /// Gets / Sets the font size of this visual text
        /// </summary>
        public float FontSize
        {
            get { return Font.Size; }
            set
            {
                if (Font.Size != value) // Avoid creating new Font resources if the size already matches
                {
                    Font = new Font(Font.FontFamily, value, Font.Style);
                }
            }
        }
        
        public StringAlignment TextHorizontalAlign
        {
            get { return _textHorizontalAlign; }
            set
            {
                _textHorizontalAlign = value;
                InvalidateTextSize();
            }
        }

        public override IGeometry Geometry
        {
            get
            {
                UpdateGeometrySize();
                return _rectangle;
            }
        }

        protected void Prototype(VisualText other)
        {
            base.Prototype(other);

            this.Text = other.Text;
            this.Font = other.Font;
            this.TextHorizontalAlign = other.TextHorizontalAlign;
        }

        #endregion

        #region Drawing

        public override void Draw(Graphics g)
        {
            var rect = GetTextRenderRect();
            var rectF = RectangleFUtil.ToRectangleF(rect);
            
            if (FillBrush != null)
            {
                g.FillRectangle(FillBrush, rectF);
            }

            if (Pen != null)
            {
                g.DrawString(Text, Font, Pen.Brush, rectF, GetStringFormat());
            }
            else
            {
                Log.Warn("Can not draw text since Pen is Null!");
            }
        }

        private StringFormat GetStringFormat()
        {
            return new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap)
            { //StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox)
                Alignment = this.TextHorizontalAlign,       // horizontal alignment
                LineAlignment = StringAlignment.Center,     // vertical alignment
                Trimming = StringTrimming.None
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the geometry with the current text size.
        /// </summary>
        private void UpdateGeometrySize()
        {
            var textSize = GetTextSize();
            _rectangle.Width = textSize.Width;
            _rectangle.Height = textSize.Height;
        }

        /// <summary>
        /// Get the rect where the text will be drawn into
        /// </summary>
        /// <returns></returns>
        /// 
        private AARectangle GetTextRenderRect()
        {
            var textSize = GetTextSize();
            return  new AARectangle(_rectangle.Location, textSize); ;
        }



        /// <summary>
        /// Invalidates all cached valuess
        /// </summary>
        private void InvalidateTextSize()
        {
            _textSizeCache = null;
        }

        /// <summary>
        /// Gets the exacte size this text with current font and settings will take
        /// </summary>
        private SizeD GetTextSize()
        {
            if (!_textSizeCache.HasValue)
            {
                var size = CalcTextSize(Text, Font, GetStringFormat());
                _textSizeCache = size;
            }
            return _textSizeCache.Value;
        }


        /// <summary>
        /// Calculates the text size
        /// </summary>
        /// <returns></returns>
        private static SizeD CalcTextSize(string text, Font font, StringFormat format)
        {
            SizeF tmpSize;
            AARectangle boundingBox;

            if (String.IsNullOrEmpty(text) || font == null)
            {
                return SizeD.Empty;
            }

            var bText = new SolidBrush(Color.Black);

            // Find the approximation of the text size
            using (var g = Graphics.FromImage(new Bitmap(100, 100)))
            {
                tmpSize = g.MeasureString(text, font);
            }

            // now create a minimal bmp with approx size
            using (var scanBMP = new Bitmap((int)tmpSize.Width, (int)tmpSize.Height))
            {
                using (var gScan = Graphics.FromImage(scanBMP))
                {
                    gScan.Clear(Color.White);
                    gScan.DrawString(text, font, bText, 1, 1, format);
                    bText.Dispose();
                }
                boundingBox = BitmapUtil.BoundingBox(scanBMP, Color.White);
            }
            return boundingBox.Size;
        }



        public override Visual Clone()
        {
            var copy = new VisualText(_rectangle.Location, _text);
            copy.Prototype(this);
            return copy;
        }

        #endregion

        public override string ToString()
        {
            return Text + " @ " + Geometry;
        }
    }
}
