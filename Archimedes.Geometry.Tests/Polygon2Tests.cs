using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Geometry.Primitives;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class Polygon2Tests
    {
        [TestCase("(10, 10), (100,10), (50,50)", 3)]
        [TestCase("(0,0), (0,0), (0,0)", 3)]
        [TestCase("(10,10), (100,234)", 2)]
        [TestCase("(10, 10), (100.2345,10), (50.1234,50.2342)", 3)]
        public void Constructor(string polygonStr, int expectedCount)
         {
             var vertices = Vector2.ParseAll(polygonStr);
             var polygon = Polygon2.Parse(polygonStr);

             Assert.AreEqual(expectedCount, polygon.VerticesCount);

             for (int i = 0; i < vertices.Length; i++)
             {
                 Assert.AreEqual(vertices[i], polygon.ToVertices()[i]);
             }
         }

        [TestCase("(10, 10), (100,10), (50,50)", "(50,20)", true)] // Triangle
        [TestCase("(10, 10), (100,10), (50,50)", "(5,5)", false)]
        [TestCase("(10, 10), (100,10), (50,50)", "(50,5)", false)]
        [TestCase("(10, 10), (100,10), (50,50)", "(50,60)", false)]
        public void Contains(string polygonStr, string pointStr, bool expected)
        {
            var polygon = Polygon2.Parse(polygonStr);
            var point = Vector2.Parse(pointStr);

            var res = polygon.Contains(point);

            Assert.AreEqual(expected, res);
        }


    }
}
