using System;
using Archimedes.Geometry.Primitives;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class Circle2Tests
    {
        
        [TestCase(100,100, 50)]
        [TestCase(0, 0, 0)]
        public void Constructor(double xs, double ys, double radius)
        {

            var c1 = new Circle2(xs, ys, radius);

            Assert.AreEqual(c1.Radius, radius);
            Assert.AreEqual(c1.MiddlePoint, new Vector2(xs, ys));
            Assert.AreEqual(c1.Area, radius * radius * Math.PI);
        }

        [TestCase(100, 100, 50, "70, 70", true)]
        [TestCase(100, 100, 50, "100, 100", true)]
        [TestCase(100, 100, 50, "10, 10", false)]
        [TestCase(0, 0, 0, "30, 20", false)]
        [TestCase(0, 0, 50, "50, 0", true)] // Point on border
        [TestCase(0, 0, 50, "51, 0", false)] // Point close away from border
        public void ContainsPoint(double xs, double ys, double radius, String point, bool expected)
        {
            var v1 = Vector2.Parse(point);
            var c1 = new Circle2(xs, ys, radius);

            Assert.True(c1.Contains(v1) == expected);
        }

    }
}
