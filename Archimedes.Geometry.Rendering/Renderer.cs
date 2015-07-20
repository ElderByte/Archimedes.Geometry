using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Diagnostics;

namespace Archimedes.Geometry.Rendering
{
    /// <summary>
    /// Threadsafe Renderer
    /// </summary>
    public class Renderer : IDisposable
    {
        #region Fields

        readonly List<DrawingGroup> _groups;
        readonly object _groupsSYNC = new object();

        readonly DrawingGroup _defaultGroup;

        Image _frame = null; // Internal Master Frame

        #endregion

        #region Events

        /// <summary>
        /// Raised before the Renderer draws its own content
        /// </summary>
        public event EventHandler<RenderEventArgs> OnPostRender;

        /// <summary>
        /// Raised after the Renderer has drawed its content
        /// </summary>
        public event EventHandler<RenderEventArgs> OnPastRender;

        #endregion

        #region Constructor

        public Renderer() {
            _groups = new List<DrawingGroup>();
            _defaultGroup = new DrawingGroup("default");
            _groups.Add(_defaultGroup);
        }

        #endregion

        #region Public Render Methods

        public void Render(int width, int height, double zoom = 1.0)
        {
            Render(new SizeD(width, height), zoom);
        }

        /// <summary>
        /// Render the IDrawable List to the master frame.
        /// </summary>
        /// <param name="frameSize"></param>
        /// <param name="zoom"></param>
        public void Render(SizeD frameSize, double zoom = 1.0) {
            if (zoom <= 0)
                throw new ArgumentException("zoom factor must not be negative!");

                Image _oldframe = _frame;
                _frame = new Bitmap(
                    (int)Math.Max(1, frameSize.Width * zoom),
                    (int)Math.Max(1, frameSize.Height * zoom));

                using (var G = Graphics.FromImage(_frame)) {
                    G.SmoothingMode = SmoothingMode.AntiAlias;
                    G.ScaleTransform((float)zoom, (float)zoom);

                    if (OnPostRender != null)
                        OnPostRender(this, new RenderEventArgs(G));

                    if (BackGroundColor.HasValue)
                        G.Clear(BackGroundColor.Value);

                    lock (_groupsSYNC) {
                        foreach (var group in _groups) {
                            group.Draw(G);
                        }
                    }

                    if (OnPastRender != null)
                        OnPastRender(this, new RenderEventArgs(G));


                if (_oldframe != null)
                    _oldframe.Dispose();
            }

        }

        /// <summary>
        /// Get Image from given ViewPort location/size
        /// </summary>
        /// <param name="viewPort"></param>
        /// <returns></returns>
        public Image GetViewPortFrame(Rectangle? viewPort = null) {
 
                Bitmap frame;

                if (_frame == null)
                    throw new NotSupportedException("Frame is null, can not get view-port!");

                if (viewPort.HasValue)
                {
                    var viewPortRect = viewPort.Value;

                    frame = new Bitmap(viewPortRect.Width, viewPortRect.Height);
                    using (Graphics g = Graphics.FromImage(frame))
                    {
                        try {
                            g.DrawImageUnscaled(_frame, viewPortRect.Location);
                        } catch(Exception e) {
                            Debug.Fail("Failed to draw image");
                        }
                    }
                } else {
                    frame = new Bitmap(_frame.Width, _frame.Height);
                    using (Graphics g = Graphics.FromImage(frame))
                    {
                        try {
                            g.DrawImageUnscaled(_frame, new Point(0, 0));
                        } catch(Exception e) {
                            Debug.Fail("Failed to draw image");
                        }
                    }
               }
                return frame;
 
        }



        #endregion

        #region Public List Handling IDrawable

        /// <summary>
        /// Clear all IDrawable Elements
        /// </summary>
        public void Clear() {
            lock (_groupsSYNC) {
                _groups.Clear();
            }
        }

        /// <summary>
        /// Add a IDrawable Element
        /// </summary>
        public void Add(IDrawable drawable, string groupname = "default") {
            GetGroupOrCreate(groupname).Add(drawable);
        }

        /// <summary>
        /// Add a range of IDrawable Elements
        /// </summary>
        public void AddRange(IEnumerable<IDrawable> udrawables, string groupname = "default") {
            GetGroupOrCreate(groupname).AddRange(udrawables);
        }

        public void Remove(IDrawable drawable, string groupname = "default") {
           //GetGroupOrDefault(groupname).Remove(drawable);
            throw new NotImplementedException();
        }

        #endregion

        public void GroupClear(string groupname) {
            lock (_groupsSYNC) {
                var grp = _groups.Find(x => x.Name.Equals(groupname));
                if (grp != null)
                    grp.Clear();
            }
        }

        private DrawingGroup GetGroupOrDefault(string groupname) {
            lock (_groupsSYNC) {
                return (from g in _groups
                        where g.Name == groupname
                        select g).DefaultIfEmpty(_defaultGroup).First();
            }
        }

        private DrawingGroup GetGroupOrCreate(string groupname) {
            lock (_groupsSYNC) {
                var grp = _groups.Find(x => x.Name.Equals(groupname));

                if (grp == null) {
                    grp = new DrawingGroup(groupname);
                    _groups.Add(grp);
                }
                return grp;
            }
        }


        #region Public Properties

        public Color? BackGroundColor {
            get;
            set;
        }

        #endregion

        public void Dispose() {
            if (_frame != null)
                _frame.Dispose();
        }
    }
}
