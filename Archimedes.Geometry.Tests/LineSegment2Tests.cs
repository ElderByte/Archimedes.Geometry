using System;
using System.Linq;
using Archimedes.Geometry.Primitives;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class LineSegment2Tests
    {
        #region Base Tests

        [TestCase("10, 10", "100, 100")]
        public void Constructor(string sp, string ep)
        {
            var spv = Vector2.Parse(sp);
            var epv = Vector2.Parse(ep);

            var l1 = new LineSegment2(spv, epv);

            Assert.AreEqual(l1.Start, spv);
            Assert.AreEqual(l1.End, epv);
        }

        [TestCase("(10,10) (100,10)", 90)]  // Horizontal
        public void Parse(string sp, double expected)
        {
            var l1 = LineSegment2.Parse(sp);
            Assert.AreEqual(expected, l1.Length);
        }


        [TestCase("10, 10", "100, 10", 90)]
        [TestCase("10, 10", "10, 10", 0)]
        [TestCase("10, 10", "10, 100", 90)]
        public void Lenght(string sp, string ep, double expected)
        {
            var spv = Vector2.Parse(sp);
            var epv = Vector2.Parse(ep);

            var l1 = new LineSegment2(spv, epv);

            Assert.AreEqual(l1.Length, expected);
        }

        #endregion

        #region Slopes / Horz & Vert

        [TestCase("10, 10", "100, 10", 0)]  // Horizontal
        [TestCase("10, 10", "10, 10", 0)]   // Only a point
        [TestCase("10, 10", "10, 100", 0)]  // Vertical
        [TestCase("0, 0", "100, 100", 1)]
        [TestCase("0, 0", "100, 200", 2)]
        public void Slope(string sp, string ep, double expected)
        {
            var spv = Vector2.Parse(sp);
            var epv = Vector2.Parse(ep);
            var l1 = new LineSegment2(spv, epv);

            Assert.AreEqual(expected, l1.Slope);
        }

        [TestCase("10, 10", "100, 10", false)]  // Horizontal
        [TestCase("10, 10", "10, 10", true)]   // Only a point
        [TestCase("10, 10", "10, 100", true)]  // Vertical
        [TestCase("0, 0", "100, 100", false)]
        [TestCase("0, 0", "100, 200", false)]
        public void IsVertical(string sp, string ep, bool expected)
        {
            var spv = Vector2.Parse(sp);
            var epv = Vector2.Parse(ep);
            var l1 = new LineSegment2(spv, epv);

            Assert.AreEqual(expected, l1.IsVertical);
        }

        [TestCase("10, 10", "100, 10", true)]  // Horizontal
        [TestCase("10, 10", "10, 10", true)]   // Only a point
        [TestCase("10, 10", "10, 100", false)]  // Vertical
        [TestCase("0, 0", "100, 100", false)]
        [TestCase("0, 0", "100, 200", false)]
        public void IsHorizontal(string sp, string ep, bool expected)
        {
            var spv = Vector2.Parse(sp);
            var epv = Vector2.Parse(ep);
            var l1 = new LineSegment2(spv, epv);

            Assert.AreEqual(expected, l1.IsHorizontal);
        }

        [TestCase("(10,10) (100,10)", "(10,10) (100,10)", true)] 
        [TestCase("(10,10) (100,10)","(10,10) (10,100)", false)]
        [TestCase("(10,10) (100,10)", "(0,500) (10,500)", true)] // Two horz lines
        [TestCase("(10,10) (10,100)", "(0,303) (0,500)", true)] // Two vert lines
        public void IsParallelTo(string line1Str, string line2Str, bool expected)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);
            Assert.AreEqual(expected, line1.IsParallelTo(line2));
        }
         

        #endregion

        #region Intersections

        [TestCase("(10, 50),(100, 50)", "(70,40),(70,60)", "(70, 50)")] // Horz - Vertical
        [TestCase("(70,40),(70,60)", "(10, 50),(100, 50)", "(70, 50)")] // Vertical - Horz
        [TestCase("(10, 50),(100, 50)", "(10,10),(90,90)", "(50, 50)")] // Horz - Diagonal
        [TestCase("(25, 567.52168),(355.95663, 567.52168)", "(212.97, 555.5),(212.97, 579.5)", "(212.97, 567.52168)")] // Specail real world case
        public void IntersectLine(string line1Str, string line2Str, string expectedIntersection)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);

            var expected = Vector2.Parse(expectedIntersection);

            var intersect = line1.HasCollision(line2);
            var intersection = line1.Intersect(line2);

            Assert.AreEqual(true, intersect);
            Assert.AreEqual(expected, intersection.First());
        }

        /*
        [TestCase("(25, 567.52168),(355.95663, 567.52168)", "(212.97, 555.5),(212.97, 579.5)", true)] // Specail real world case
        public void IntersectLine(string line1Str, string line2Str, bool expectedIntersection)
        {
            var line1 = Line2.Parse(line1Str);
            var line2 = Line2.Parse(line2Str);

            var intersection = line1.Intersect(line2);
            var intersects = line1.IntersectsWith(line2);

            Assert.AreEqual(expectedIntersection, intersects);
        }*/




        [TestCase("(10,50) (100,50)", "(30,30)"," (50,40)", "(30,50),(80,50)")] // Horz. Line with two intersections
        [TestCase("(10,50) (50,50)", "(30,30)", " (50,40)", "(30,50)")]         // Horz. Line with one intersection
        [TestCase("(10,50) (25,50)", "(30,30)", " (50,40)", "")]         // Horz. Line with no intersection
        [TestCase("(10,10) (90,90)", "(30,30)", " (50,40)", "(30,30),(70,70)")] // Diagonal Line with two intersections
       // [TestCase("(25, 567.521681),(355.95663, 567.521681)", "(167.97, 555.5)", "(45, 24)", "(11,11)")] // Case from real data

        public void InterceptRect(string line1Str, string rectLocationStr, string rectSizeStr, string expectedIntersections)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var rect = new AARectangle(Vector2.Parse(rectLocationStr), SizeD.Parse(rectSizeStr));

            var expectedInters = Vector2.ParseAll(expectedIntersections);

            var actual = line1.InterceptRect(rect);

            Assert.AreEqual(expectedInters.Length, actual.Count); // Expected number of intersections
            // Check each point
            if (expectedInters.Length == actual.Count)
            {
                for (int i = 0; i < expectedInters.Length; i++)
                {
                    Assert.AreEqual(expectedInters[i], actual[i]);
                }
            }
        }

        #endregion
    }
}
