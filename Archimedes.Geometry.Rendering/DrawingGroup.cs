using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace Archimedes.Geometry.Rendering
{
    /// <summary>
    /// Threadsafe Drawing Group
    /// </summary>
    public class DrawingGroup : IDrawable
    {
        #region Fields

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<IDrawable> _grpdrawings = new List<IDrawable>();
        private readonly object _drawingsSync = new object();

        #endregion

        public string Name { get; set; }

        public DrawingGroup(string uname) {
            Name = uname;
        }

        #region Drawable Handling

        public void Add(IDrawable drawable)
        {
            if (drawable == null) throw new ArgumentNullException("drawable"); 
            lock (_drawingsSync) {
                _grpdrawings.Add(drawable);
            }
        }

        public void AddRange(IEnumerable<IDrawable> drawables) {
            lock (_drawingsSync) {
                foreach (var drawable in drawables)
                {
                    if (drawable != null)
                    {
                        Add(drawable);
                    }
                }
            }
        }


        /// <summary>
        /// Remove the given Element
        /// </summary>
        /// <param name="d"></param>
        public void Remove(IDrawable d) {
            lock (_drawingsSync) {
                _grpdrawings.Remove(d);
            }
        }

        /// <summary>
        /// Clear all Elements
        /// </summary>
        public void Clear() {
            lock (_drawingsSync) {
                _grpdrawings.Clear();
            }
        }

        /// <summary>
        /// Element Count
        /// </summary>
        public int Count {
            get {
                lock (_drawingsSync) {
                    return _grpdrawings.Count;
                }
            }
        }

        /// <summary>
        /// Get an immutable snapshot of all Drawing Elements currently contained in this Group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDrawable> GetSnapshot() {
            lock (_drawingsSync) {
                return new List<IDrawable>(_grpdrawings);
            }
        }

        #endregion


        /// <summary>
        /// Draws all Elements to the given Gfx Context
        /// </summary>
        /// <param name="g"></param>
        public void Draw(System.Drawing.Graphics g) {
            foreach (var d in GetSnapshot()) {
                try {
                    d.Draw(g);
                } catch (Exception e) {
                    Log.Warn("Could not draw " + d + "!", e);
                }
            }
        }
    }
}
