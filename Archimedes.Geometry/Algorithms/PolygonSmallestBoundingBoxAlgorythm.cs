using System;
using System.Linq;
using Archimedes.Geometry.Primitives;

namespace Archimedes.Geometry.Algorithms
{
    /// <summary>
    /// Algorythm which tries to find the smallest (Aera) BoundingBox Rectangle of the given Polygon.
    /// 
    /// </summary>
    public class PolygonSmallestBoundingBoxAlgorythm : IPolygonBoundingBoxAlgorythm
    {
        #region Fields

        private int _numPoints = 0;

        // The points that have been used in test edges.
        private bool[] _edgeChecked = null;

        // The four caliper control points. They start:
        //       m_ControlPoints(0)      Left edge       xmin
        //       m_ControlPoints(1)      Bottom edge     ymax
        //       m_ControlPoints(2)      Right edge      xmax
        //       m_ControlPoints(3)      Top edge        ymin
        private readonly int[] _controlPoints = new int[4];

        // The line from this point to the next one forms
        // one side of the next bounding rectangle.
        private int _currentControlPoint = -1;
        private Vector2[] _currentRectangle;

        // The area of the current and best bounding rectangles.
        //private double _currentArea = double.MaxValue;
        private double _bestArea = double.MaxValue;
        private Vector2[] _bestRectangle = null;

        #endregion

        #region Constructor

        public PolygonSmallestBoundingBoxAlgorythm() { }


        #endregion

        #region Public Methods

        /// <summary>
        /// Find a smallest bounding rectangle.
        /// </summary>
        /// <returns></returns>
        public Vector2[] FindBounds(Polygon2 polygon){

            if (polygon == null) throw new ArgumentNullException("polygon");

            var minified = new Polygon2(polygon.ToVertices().Distinct());
            minified.OrientCounterClockwise();
            var vertices = minified.ToVertices().ToArray();

            if (vertices.Count() == 0)
            {
                return new Vector2[0];
            }

            // This algorithm assumes the polygon is oriented counter-clockwise.

            // Get ready;
            ResetBoundingRect(vertices);

            // Check all possible bounding rectangles.
            for (int i = 0; i < vertices.Count(); i++)
            {
                CheckNextBoundingRectangle(vertices);
            }

            // Return the best result.
            return _bestRectangle;
        }

        #endregion

        #region Private Methods
        // Get ready to start.
        private void ResetBoundingRect(Vector2[] vertices)
        {
            _numPoints = vertices.Count();
            // Find the minimum and maximum points
            // in all four directions.
            double minx = vertices[0].X;
            double maxx = minx;
            double miny = vertices[0].Y;
            double maxy = miny;
            double minxi = 0;
            double maxxi = 0;
            double minyi = 0;
            double maxyi = 0;
            for (int i = 1; i < _numPoints; i++)
            {
                if (minx > vertices[i].X)
                {
                    minx = vertices[i].X;
                    minxi = i;
                }
                if (maxx < vertices[i].X)
                {
                    maxx = vertices[i].X;
                    maxxi = i;
                }
                if (miny > vertices[i].Y)
                {
                    miny = vertices[i].Y;
                    minyi = i;
                }
                if (maxy < vertices[i].Y)
                {
                    maxy = vertices[i].Y;
                    maxyi = i;
                }
            }
            _controlPoints[0] = (int)minxi;
            _controlPoints[1] = (int)maxyi;
            _controlPoints[2] = (int)maxxi;
            _controlPoints[3] = (int)minyi;
            _currentControlPoint = -1;
            // Reset the current and best bounding rectangle.
            _currentRectangle = new Vector2[]
            {
            new Vector2(minx, miny),
            new Vector2(maxx, miny),
            new Vector2(maxx, maxy),
            new Vector2(minx, maxy),
            };

            var currentArea = (maxx - minx) * (maxy - miny);
            _bestRectangle = _currentRectangle;
            _bestArea = currentArea;
            // So far we have not checked any edges.
            _edgeChecked = new bool[_numPoints];
            for (int i = 0; i < _numPoints; i++)
            {
                _edgeChecked[i] = false;
            }
        }
        // Find the next bounding rectangle and check it.
        private void CheckNextBoundingRectangle(Vector2[] vertices)
        {
            // Increment the current control point.
            // This means we are done with using this edge.
            if (_currentControlPoint >= 0)
            {
                _controlPoints[_currentControlPoint] =
                (_controlPoints[_currentControlPoint] + 1) % _numPoints;
            }
            // Find the next point on an edge to use.
            double xmindx = 0, xmindy = 0, ymaxdx = 0, ymaxdy = 0;
            double xmaxdx = 0, xmaxdy = 0, ymindx = 0, ymindy = 0;
            FindDxDy(vertices, ref xmindx, ref xmindy, _controlPoints[0]);
            FindDxDy(vertices, ref ymaxdx, ref ymaxdy, _controlPoints[1]);
            FindDxDy(vertices, ref xmaxdx, ref xmaxdy, _controlPoints[2]);
            FindDxDy(vertices, ref ymindx, ref ymindy, _controlPoints[3]);
            // Switch so we can look for the smallest opposite/adjacent ratio.
            var xminopp = xmindx;
            var xminadj = xmindy;
            var ymaxopp = -ymaxdy;
            var ymaxadj = ymaxdx;
            var xmaxopp = -xmaxdx;
            var xmaxadj = -xmaxdy;
            var yminopp = ymindy;
            var yminadj = -ymindx;
            // Pick initial values that will make every point an improvement.
            double bestopp = 1;
            double bestadj = 0;
            int best_control_point = -1;
            // Pick the best control point to use next.
            if ((xminopp >= 0) && (xminadj >= 0))
            {
                if (xminopp * bestadj < bestopp * xminadj)
                {
                    bestopp = xminopp;
                    bestadj = xminadj;
                    best_control_point = 0;
                }
            }
            if ((ymaxopp >= 0) && (ymaxadj >= 0))
            {
                if (ymaxopp * bestadj < bestopp * ymaxadj)
                {
                    bestopp = ymaxopp;
                    bestadj = ymaxadj;
                    best_control_point = 1;
                }
            }
            if ((xmaxopp >= 0) && (xmaxadj >= 0))
            {
                if (xmaxopp * bestadj < bestopp * xmaxadj)
                {
                    bestopp = xmaxopp;
                    bestadj = xmaxadj;
                    best_control_point = 2;
                }
            }
            if ((yminopp >= 0) && (yminadj >= 0))
            {
                if (yminopp * bestadj < bestopp * yminadj)
                {
                    bestopp = yminopp;
                    bestadj = yminadj;
                    best_control_point = 3;
                }
            }
            // Make sure we found a usable edge.
            if (best_control_point < 0)
            {
                return;
            }
            // Use the new best control point.
            _currentControlPoint = best_control_point;
            // Remember that we have checked this edge.
            if (_currentControlPoint != -1)
                _edgeChecked[_controlPoints[_currentControlPoint]] = true;
            // Find the current bounding rectangle
            // and see if it is an improvement.
            FindBoundingRectangle(vertices);
        }
        /// <summary>
        /// Find the current bounding rectangle and
        /// see if it is better than the previous best.
        /// </summary>
        private void FindBoundingRectangle(Vector2[] vertices)
        {
            // See which point has the current edge.
            int i1 = _controlPoints[_currentControlPoint];
            int i2 = (i1 + 1) % _numPoints;
            var dx = vertices[i2].X - vertices[i1].X;
            var dy = vertices[i2].Y - vertices[i1].Y;
            // Make dx and dy work for the first line.
            switch (_currentControlPoint)
            {
                case 0: // Nothing to do.
                    break;
                case 1: // dx = -dy, dy = dx
                    var temp1 = dx;
                    dx = -dy;
                    dy = temp1;
                    break;
                case 2: // dx = -dx, dy = -dy
                    dx = -dx;
                    dy = -dy;
                    break;
                case 3: // dx = dy, dy = -dx
                    var temp2 = dx;
                    dx = dy;
                    dy = -temp2;
                    break;
            }
            var px0 = vertices[_controlPoints[0]].X;
            var py0 = vertices[_controlPoints[0]].Y;
            var dx0 = dx;
            var dy0 = dy;
            var px1 = vertices[_controlPoints[1]].X;
            var py1 = vertices[_controlPoints[1]].Y;
            var dx1 = dy;
            var dy1 = -dx;
            var px2 = vertices[_controlPoints[2]].X;
            var py2 = vertices[_controlPoints[2]].Y;
            var dx2 = -dx;
            var dy2 = -dy;
            var px3 = vertices[_controlPoints[3]].X;
            var py3 = vertices[_controlPoints[3]].Y;
            var dx3 = -dy;
            var dy3 = dx;
            // Find the points of intersection.
            _currentRectangle = new Vector2[4];
            FindIntersection(px0, py0, px0 + dx0, py0 + dy0, px1, py1, px1 + dx1, py1 + dy1, ref _currentRectangle[0]);
            FindIntersection(px1, py1, px1 + dx1, py1 + dy1, px2, py2, px2 + dx2, py2 + dy2, ref _currentRectangle[1]);
            FindIntersection(px2, py2, px2 + dx2, py2 + dy2, px3, py3, px3 + dx3, py3 + dy3, ref _currentRectangle[2]);
            FindIntersection(px3, py3, px3 + dx3, py3 + dy3, px0, py0, px0 + dx0, py0 + dy0, ref _currentRectangle[3]);
            if (IsCurrentRectangleTheBest(_currentRectangle))
            {
                _bestRectangle = _currentRectangle;
            }
        }
        /// <summary>
        /// See if this is the best bounding rectangle so far.
        /// Get the area of the bounding rectangle.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCurrentRectangleTheBest(Vector2[] currentRectangle)
        {
            var vx0 = currentRectangle[0].X - currentRectangle[1].X;
            var vy0 = currentRectangle[0].Y - currentRectangle[1].Y;
            var len0 = Math.Sqrt(vx0 * vx0 + vy0 * vy0);
            var vx1 = currentRectangle[1].X - currentRectangle[2].X;
            var vy1 = currentRectangle[1].Y - currentRectangle[2].Y;
            var len1 = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
            
            // See if this is an improvement.
            var currentArea = len0 * len1;
            if (currentArea < _bestArea)
            {
                _bestArea = currentArea;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Find the slope of the edge from point i to point i + 1.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="i"></param>
        private void FindDxDy(Vector2[] vertices, ref double dx, ref double dy, int i)
        {
            int i2 = (i + 1) % _numPoints;
            dx = vertices[i2].X - vertices[i].X;
            dy = vertices[i2].Y - vertices[i].Y;
        }
        /// <summary>
        /// Find the point of intersection between two lines.
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        /// <param name="A1"></param>
        /// <param name="B1"></param>
        /// <param name="A2"></param>
        /// <param name="B2"></param>
        /// <param name="intersect"></param>
        /// <returns></returns>
        private bool FindIntersection(double X1, double Y1, double X2, double Y2, double A1, double B1, double A2, double B2, ref Vector2 intersect)
        {
            var dx = X2 - X1;
            var dy = Y2 - Y1;
            var da = A2 - A1;
            var db = B2 - B1;
            double s, t;
            // If the segments are parallel, return False.
            if (Math.Abs(da * dy - db * dx) < 0.001) return false;
            // Find the point of intersection.
            s = (dx * (B1 - Y1) + dy * (X1 - A1)) / (da * dy - db * dx);
            t = (da * (Y1 - B1) + db * (A1 - X1)) / (db * dx - da * dy);
            intersect = new Vector2(X1 + t * dx, Y1 + t * dy);
            return true;
        }

        #endregion
    }
}
