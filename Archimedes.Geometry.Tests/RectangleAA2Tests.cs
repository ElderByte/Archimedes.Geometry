using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Archimedes.Geometry.Primitives;
using Archimedes.Geometry.Units;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class RectangleAA2Tests
    {
        [TestCase("100, 100", "500, 300")]
        [TestCase("0, 0", "50, 50")]
        public void Constructor(string locationStr, string sizeStr)
        {


            var location = Vector2.Parse(locationStr);
            var size = SizeD.Parse(sizeStr);

            var rect = new RectangleAA2(location, size);

            Assert.AreEqual(location, rect.Location);
            Assert.AreEqual(size.Width, rect.Width);
            Assert.AreEqual(size.Height, rect.Height);
        }

        [TestCase]
        public void EmptyTests()
        {
            var rect = RectangleAA2.Empty;

            // Check the empty rect
            Assert.AreEqual(Vector2.Zero, rect.Location);
            Assert.AreEqual(0, rect.Width);
            Assert.AreEqual(0, rect.Height);

            // Check how changes to the empty behave

            rect.Location = new Vector2(100, 100);
            // Check the empty rect
            Assert.AreEqual(new Vector2(100, 100), rect.Location);
            Assert.AreEqual(0, rect.Width);
            Assert.AreEqual(0, rect.Height);


            rect = RectangleAA2.Empty;
            rect.Width = 100;
            rect.Height = 50;

            Assert.AreEqual(Vector2.Zero, rect.Location);
            Assert.AreEqual(100, rect.Width);
            Assert.AreEqual(50, rect.Height);
        }

        [TestCase]
        public void ToLinesTest()
        {
            var rect = new RectangleAA2(new Vector2(10,10), new SizeD(20,20));

            var lines = rect.ToLines();

            var bottom = new LineSegment2(new Vector2(10, 10), new Vector2(30, 10));
            var top = new LineSegment2(new Vector2(30, 30), new Vector2(10, 30));
            var right = new LineSegment2(new Vector2(30, 10), new Vector2(30, 30));
            var left = new LineSegment2(new Vector2(10, 30), new Vector2(10, 10));

            Assert.True((from l in lines where l.Equals(bottom) select l).Any());
            Assert.True((from l in lines where l.Equals(top) select l).Any());
            Assert.True((from l in lines where l.Equals(right) select l).Any());
            Assert.True((from l in lines where l.Equals(left) select l).Any());
        }



        [TestCase("10, 10", "10, 10", "(5, 15),(45, 15)", true)] // intersecting it
        [TestCase("10, 10", "10, 10", "(5, 30),(45, 30)", false)] // above it
        [TestCase("10, 10", "10, 10", "(5, 5),(45, 5)", false)]  // under it
        [TestCase("10, 10", "10, 10", "(30, 15),(45, 15)", false)] // right of it
        [TestCase("10, 10", "10, 10", "(0, 15),(5, 15)", false)] // left of it
        public void CollisionWithLine(string locationStr, string sizeStr, string lineStr, bool expectedCollision)
        {
            var location = Vector2.Parse(locationStr);
            var size = SizeD.Parse(sizeStr);
            var rect = new RectangleAA2(location, size);

            var line = LineSegment2.Parse(lineStr);


            var hasCollision = rect.HasCollision(line);

            Assert.AreEqual(expectedCollision, hasCollision);

        }

    }
}
