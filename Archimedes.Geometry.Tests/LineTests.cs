using System;
using System.Collections.Generic;
using Archimedes.Geometry.Primitives;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class LineTests
    {

        [TestCase("10, 10", "1, 0")]
        [TestCase("10, 10", "0, 0")]
        [TestCase("10, 10", "3, 2")]
        public void Constructor(string locationStr, string directionStr)
        {
            var location = Vector2.Parse(locationStr);
            var direction = Vector2.Parse(directionStr);

            var l1 = new Line(location, direction);

            Assert.AreEqual(l1.Location, location);
            Assert.AreEqual(l1.Direction, direction);
        }


        [TestCase("0, 0", "0, 1", 0)]   // Vertical
        [TestCase("1, 1", "0, 20", 0)]   // Vertical
        public void TestYIntersect(string locationStr, string directionStr, double expectedYIntersect)
        {
            var location = Vector2.Parse(locationStr);
            var direction = Vector2.Parse(directionStr);

            var l1 = new Line(location, direction);
            Assert.AreEqual(expectedYIntersect, l1.IntersectY);
        }


        [TestCase("0, 0", "1, 0", "20, 0", true)]   // Horizontal
        [TestCase("0, 0", "1, 0", "20, 1", false)]
        [TestCase("0, 0", "0, 1", "0, 20", true)]   // Vertical
        [TestCase("55, 0", "0, 1", "55, 20", true)]   // Vertical
        [TestCase("0, 0", "0, 1", "1, 20", false)]
        [TestCase("0, 0", "1, 1", "345, 345", true)]
        [TestCase("0, 0", "1, 1", "-666, -666", true)]
        [TestCase("0, 0", "1, 1", "-662, -666", false)]
        [TestCase("22, 22", "1, 1", "345, 345", true)]
        [TestCase("-22, -22", "1, 1", "345, 345", true)]
        [TestCase("10, 20", "1, 1", "10, 20", true)]
        [TestCase("10, 20", "1, 1", "10, 30", false)]
        [TestCase("10, 20", "1, 1", "60, 70", true)]
        public void ContainsPoint(string locationStr, string directionStr, string pointStr, bool expectedContains)
        {
            var location = Vector2.Parse(locationStr);
            var direction = Vector2.Parse(directionStr);
            var point = Vector2.Parse(pointStr);


            var l1 = new Line(location, direction);
            var contains = l1.Contains(point);
            Assert.AreEqual(expectedContains, contains);
        }


        [TestCase("0, 0", "1, 1", "0, 60", "-1,1", "30,30")]
        [TestCase("0, 20", "1, 0", "0, 60", "-1,1", "40,20")] // Horz with diagonal
        [TestCase("20, 0", "0, 1", "0, 60", "-1,1", "20,40")] // Vert with diagonal

        public void Intersection(string l1LocationStr, string l1directionStr, string l2LocationStr, string l2directionStr, string expectedIntersectionStr)
        {
            var l1 = new Line(Vector2.Parse(l1LocationStr),  Vector2.Parse(l1directionStr));
            var l2 = new Line(Vector2.Parse(l2LocationStr), Vector2.Parse(l2directionStr));
            var expectedIntersection = Vector2.Parse(expectedIntersectionStr);

            var actual = l1.Intersection(l2);

            Assert.AreEqual(expectedIntersection, actual);
        }



    }
}
