using System;
using System.Collections.Generic;
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

        [TestCase("(10, 20),(200, 20)", "(10, 20),(200, 20)")]
        public void TestEquality(string lineAStr, string lineBStr)
        {
            var lineA = LineSegment2.Parse(lineAStr);
            var lineB = LineSegment2.Parse(lineBStr);

            Assert.AreEqual(lineA, lineB);
        }

        [TestCase("(10, 20),(200, 20)", "(10, 20),(200, 20)")]
        public void TestEqualityInList(string lineAStr, string lineBStr)
        {
            var lineA = LineSegment2.Parse(lineAStr);
            var lineB = LineSegment2.Parse(lineBStr);

            var list = new List<LineSegment2>();
            list.Add(lineA);

            Assert.IsTrue(list.Contains(lineB));
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

        #region Collisions

        [TestCase("(5, 30),(45, 30)", "(10,10),(20,10)", false)] // No collision (lines parallel above each other)
        [TestCase("(10, 50),(100, 50)", "(70,40),(70,60)", true)] // Horz - Vertical
        [TestCase("(70,40),(70,60)", "(10, 50),(100, 50)", true)] // Vertical - Horz
        [TestCase("(10, 50),(100, 50)", "(10,10),(90,90)", true)] // Horz - Diagonal
        [TestCase("(25, 567.52168),(355.95663, 567.52168)", "(212.97, 555.5),(212.97, 579.5)", true)] // Specail real world case
        [TestCase("(10, 100),(10, 20)", "(10,20),(150,20)", true)] // Two lines meet in a point - collision
        [TestCase("(10, 20),(130, 20)", "(100,20),(200,20)", true)] // Paralel lines overlap - large collision
        [TestCase("(10, 20),(60, 70)", "(30,20),(20,20)", false)]
        public void TestLineCollisions(string line1Str, string line2Str, bool expectedCollision)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);

            var collides = line1.HasCollision(line2);
            Assert.AreEqual(expectedCollision, collides);
        }

        #endregion

        #region Intersections


        [TestCase("(10, 50),(100, 50)", "(70,40),(70,60)", true)] // Horz - Vertical
        [TestCase("(70,40),(70,60)", "(10, 50),(100, 50)", true)] // Vertical - Horz
        [TestCase("(10, 50),(100, 50)", "(10,10),(90,90)", true)] // Horz - Diagonal
        [TestCase("(25, 567.52168),(355.95663, 567.52168)", "(212.97, 555.5),(212.97, 579.5)", true)] // Specail real world case
        [TestCase("(10, 100),(10, 20)", "(10,20),(150,20)", false)] // // Two lines meet - no proper intersection
        [TestCase("(10, 20),(130, 20)", "(100,20),(200,20)", false)] // Paralel lines overlap - no proper intersection
        [TestCase("(34, 546),(12, 132)", "(12,132),(4421,2354)", false)] // // Two lines meet in end points - no proper intersection
        [TestCase("(10, 20),(10, 120)", "(10,50),(50,250)", false)] // // A line touches another - no proper intersection

        public void TestProperIntersection(string line1Str, string line2Str, bool expectedProperIntersection)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);
            var properIntersection = line1.IsIntersectionProper(line2);
            Assert.AreEqual(expectedProperIntersection, properIntersection);
        }


        [TestCase("(10, 50),(100, 50)", "(70,40),(70,60)", "(70, 50)")] // Horz - Vertical
        [TestCase("(70,40),(70,60)", "(10, 50),(100, 50)", "(70, 50)")] // Vertical - Horz
        [TestCase("(10, 50),(100, 50)", "(10,10),(90,90)", "(50, 50)")] // Horz - Diagonal
        [TestCase("(25, 567.52168),(355.95663, 567.52168)", "(212.97, 555.5),(212.97, 579.5)", "(212.97, 567.52168)")] // Specail real world case
        public void IntersectLine(string line1Str, string line2Str, string expectedIntersection)
        {
            IGeometry line1 = LineSegment2.Parse(line1Str);
            IGeometry line2 = LineSegment2.Parse(line2Str);

            var expected = Vector2.Parse(expectedIntersection);

            var intersection = line1.Intersect(line2);

            Assert.AreEqual(expected, intersection.First());
        }


        [TestCase]
        public void InterceptRectNo()
        {
            var line1 = LineSegment2.Parse("(10, 5000)(2000, 5000)");
            var rect = new AARectangle(new Vector2(30,30), new SizeD(50,40));

            var actual = line1.RectIntersection(rect);

            Assert.AreEqual(false, line1.HasRectIntersection(rect));
            Assert.AreEqual(0, actual.Count);
        }


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

            var actual = line1.RectIntersection(rect);

            Assert.AreEqual(expectedInters.Length, actual.Count); // Expected number of intersections
            // Check each point
            if (expectedInters.Length == actual.Count)
            {
                foreach (var p in actual)
                {
                    Assert.True(expectedInters.Contains(p));
                }
            }
        }

        #endregion

        #region Overlaps

        [TestCase("(25, 175) (100, 175)", "(25,175) (175, 175)", true)]
        [TestCase("(25, 174.99999999999997) (99.999999999999986, 175)", "(25.000000000000028,175) (175, 175)", true)]
        public void TesthasOverlap(string line1Str, string line2Str, bool expectedOverlap)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);
            var hasOverlap = line1.HasOverlap(line2);
            Assert.AreEqual(expectedOverlap, hasOverlap);
        }


        [TestCase("(10, 20),(200, 20)", "(50,20),(300,20)", "(50,20),(200,20)")]        // Partially overlapping
        [TestCase("(10, 20),(200, 20)", "(10, 20),(200, 20)", "(10, 20),(200, 20)")]    // Fully overlapping
        [TestCase("(10, 10),(200, 10)", "(10, 10),(50, 10)", "(10, 10),(50, 10)")]
        public void TestOverlap(string line1Str, string line2Str, string expectedOverlapStr)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);
            var expectedOverlap = LineSegment2.Parse(expectedOverlapStr);

            var overlap = line1.GetOverlapSegment(line2);
            Assert.AreEqual(expectedOverlap, overlap);
        }

        [TestCase("(10, 20),(200, 20)", "(50,20),(300,30)")]  // Not parallel - no overlap, at best an intersection
        [TestCase("(10, 20),(200, 20)", "(300,20),(400,20)")] // They are disjoint
        [TestCase("(10, 20),(200, 20)", "(200,20),(400,20)")] // They end-start meet in one point
        [TestCase("(5, 30),(45, 30)", "(10,10),(20,10)")]     // Parallel but line hovers above other
        [TestCase("(10, 20),(60, 70)", "(30,20),(20,20)")]    // non axial line 
        [TestCase("(10, 20),(60, 70)", "(35,30),(45,40)")]    // two parallel but nonaxial non overlapping lines 
        public void TestNoOverlap(string line1Str, string line2Str)
        {
            var line1 = LineSegment2.Parse(line1Str);
            var line2 = LineSegment2.Parse(line2Str);

            var overlap = line1.HasOverlap(line2);
            Assert.AreEqual(false, overlap);
        }

        #endregion
    }
}
