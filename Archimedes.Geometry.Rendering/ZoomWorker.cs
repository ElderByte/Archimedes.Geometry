using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archimedes.Geometry.Rendering
{
    public class ZoomControler
    {
        private double _zoomMultiplier = 1.5f;
        private double _currentZoom = 1.0f;

        public ZoomControler() { }

        public ZoomControler(float multiplier) {
            ZoomMultiplier = multiplier;
        }

        public void ZoomIn() {
            _currentZoom *= (1 * _zoomMultiplier);
        }
        public void ZoomOut() {
            _currentZoom *= (1 / _zoomMultiplier); 
        }

        public double CurrentZoom {
            get { return _currentZoom; }
        }
        public double ZoomMultiplier
        {
            get { return _zoomMultiplier; }
            set {
                if (value <= 0)
                    throw new ArgumentException("multiplier must be greater than Null");
                _zoomMultiplier = value; 
            }
        }

    }
}
