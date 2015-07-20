using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Rendering.Primitives
{
    public class VisualImage : Visual
    {
        private readonly RectangleAA2 _imageRect;
        private Image _image;

        public VisualImage()
            : this(Vector2.Zero, null)
        {
        }

        public VisualImage(Vector2 location, Image image)
        {
            _imageRect = new RectangleAA2(location.X, location.Y, 0, 0);
            Image = image;
        }

        public Image Image
        {
            get { return _image; }

            set
            {
                _image = value;
                if (_image != null)
                {
                    _imageRect.Width = _image.Width;
                    _imageRect.Height = _image.Height;
                }
                else
                {
                    _imageRect.Width = 0;
                    _imageRect.Height = 0;
                }
            }
        }


        public override IGeometry Geometry
        {
            get { return _imageRect; }
        }

        public override void Draw(Graphics g)
        {
            if (_image != null)
            {
                var drawRect = new RectangleF(PointFUtil.ToPointF(_imageRect.Location), new SizeF((float)_imageRect.Width, (float)_imageRect.Height));
                g.DrawImage(Image, drawRect);
            }
        }

        public override Visual Clone()
        {
            return new VisualImage(_imageRect.Location, _image);
        }
    }
}
