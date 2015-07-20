using System;
using Archimedes.Geometry.Units;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class VectorTests
    {

        [TestCase("(1, 0)", 1)]
        [TestCase("(1, 0) (1, 0)", 2)]
        [TestCase("(1, 0) (1, 0) (3,6)", 3)]
        public void Parse(string v1s, int expected)
        {
            var vertices = Vector2.ParseAll(v1s);
            Assert.AreEqual(vertices.Length, expected);
        }




        [TestCase("1, 0", "1, 0", 1e-4, true)]
        [TestCase("-1, 1", "-1, 1", 1e-4, true)]
        [TestCase("1, 0", "1, 1", 1e-4, false)]
        public void Equals(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            Assert.AreEqual(expected, v1 == v2);
            Assert.AreEqual(expected, v1.Equals(v2));
            Assert.AreEqual(expected, v1.Equals((object) v2));
            Assert.AreEqual(expected, Equals(v1, v2));
            Assert.AreEqual(expected, v1.Equals(v2, tol));
            Assert.AreNotEqual(expected, v1 != v2);
        }

        [TestCase("-1, -2", "1, 2", "0, 0")]
        public void Add(string v1s, string v2s, string evs)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            var actuals = new[]
            {
                v1 + v2,
                v2 + v1,
            };
            var expected = Vector2.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", "1, 2", "-2, -4")]
        public void Subtract(string v1s, string v2s, string evs)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            var actuals = new[]
            {
                v1 - v2,
            };
            var expected = Vector2.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }


        [TestCase("-1, -2", 2, "-2, -4")]
        public void MultiplyAndScaleBy(string vs, double d, string evs)
        {
            var v = Vector2.Parse(vs);
            var actuals = new[]
                          {
                              d * v,

                          };
            var expected = Vector2.Parse(evs);
            foreach (var actual in actuals)
            {
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("-1, -2", 2, "-0.5, -1")]
        public void Divide(string vs, double d, string evs)
        {
            var v = Vector2.Parse(vs);
            var actual = v / d;
            var expected = Vector2.Parse(evs);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("2, 0", 2)]
        [TestCase("-2, 0", 2)]
        [TestCase("0, 2", 2)]
        public void Length(string vs, double expected)
        {
            var v = Vector2.Parse(vs);
            Assert.AreEqual(expected, v.Length, 1e-6);
        }


        [TestCase("1, 0", "1, 0", 1e-4, false)]
        [TestCase("1, 0", "0, -1", 1e-4, true)]
        [TestCase("1, 0", "0, 1", 1e-4, true)]
        [TestCase("0, 1", "1, 0", 1e-4, true)]
        [TestCase("0, 1", "0, 1", 1e-4, false)]
        public void IsPerpendicularTo(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            Assert.AreEqual(expected, v1.IsPerpendicularTo(v2, tol));
            Assert.AreEqual(expected, v2.IsPerpendicularTo(v1, tol));
        }

        [TestCase("1, 0", "1, 0", 1e-4, true)]
        [TestCase("1, 0", "-1, 0", 1e-4, true)]
        [TestCase("1, 0", "1, 1", 1e-4, false)]
        [TestCase("1, 1", "1, 1", 1e-4, true)]
        [TestCase("1, -1", "-1, 1", 1e-4, true)]
        [TestCase("-100, 260", "-39.321608040201028, 102.2361809045226", 1e-4, true)]
        public void IsParallelTo(string v1s, string v2s, double tol, bool expected)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            Assert.AreEqual(expected, v1.IsParallelTo(v2, tol));
            Assert.AreEqual(expected, v2.IsParallelTo(v1, tol));
        }

        [TestCase("1, 0", "0, 1", 90)]
        public void LegacyAngleToTest(string v1s, string v2s, float expected)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);

            Angle av = v1.GetAngleTo(v2);
            Assert.AreEqual(expected, av.Degrees, 0.1);
        }


        [TestCase("1, 0",  0)]
        [TestCase("0, 1", 90)]
        [TestCase("1, 1", 45)]
        public void LegacyAngleToXTest(string v1s, float expected)
        {
            var v1 = Vector2.Parse(v1s);

            Angle av = v1.AngleSignedTo(Vector2.UnitX, true);
            Assert.AreEqual(expected, av.Degrees, 0.1);
        }


        [TestCase("1,0", "0,1", "270°", "-90°")]
        [TestCase("0,1", "1,0", "90°", "90°")]
        [TestCase("-0.99985, 0.01745", "-1, 0", "359°", "-1°")]
        [TestCase("-0.99985, -0.01745", "-1, 0", "1°", "1°")]
        [TestCase("0.99985, 0.01745", "1, 0", "1°", "1°")]
        [TestCase("0.99985, -0.01745", "1, 0", "359°", "-1°")]
        public void SignedAngleTo(string v1s, string v2s, string expectedClockWise, string expectedNegative)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);
            var cw = v1.AngleSignedTo(v2, true);
            var expected = Angle.Parse(expectedClockWise);
            Assert.AreEqual(expected.Degrees, cw.Degrees, 1e-3);
            var cwNeg = v1.AngleSignedTo(v2, true, true);
            Assert.AreEqual(Angle.Parse(expectedNegative).Degrees, cwNeg.Degrees, 1e-3);

            var ccw = v1.AngleSignedTo(v2, false);
            Assert.AreEqual(360 - expected.Degrees, ccw.Degrees, 1e-3);
        }

        [TestCase("1,0", "0,1", "270°")]
        [TestCase("0,1", "1,0", "90°")]
        [TestCase("-0.99985, 0.01745", "-1, 0", "359°")]
        [TestCase("-0.99985, -0.01745", "-1, 0", "1°")]
        [TestCase("0.99985, 0.01745", "1, 0", "1°")]
        [TestCase("0.99985, -0.01745", "1, 0", "359°")]
        public void SignedAngleTo(string v1s, string v2s, string expectedClockWise)
        {
            var v1 = Vector2.Parse(v1s);
            var v2 = Vector2.Parse(v2s);

            var cw = v1.AngleSignedTo(v2, true);
            var expected = Angle.Parse(expectedClockWise);
            Assert.AreEqual(expected.Degrees, cw.Degrees, 1e-3);
        }



        [TestCase("1, 0", "90°", "0, 1")]
        [TestCase("1, 0", "-270°", "0, 1")]
        [TestCase("1, 0", "-90°", "0, -1")]
        [TestCase("1, 0", "270°", "0, -1")]
        [TestCase("1, 0", "180°", "-1, 0")]
        [TestCase("1, 0", "180°", "-1, 0")]
        [TestCase("1, 0", "0°", "1, 0")]
        [TestCase("0, 1", "-90°", "1, 0")]
        public void Rotate(string vs, string @as, string evs)
        {
            var v = Vector2.Parse(vs);
            var angle = Angle.Parse(@as);

            var actual = v.GetRotated(angle);
            var expected = Vector2.Parse(evs);

            Assert.True(expected.Equals(actual, 0.01));
        }

        [TestCase("1, 2", "3, 4", 11)]
        public void DotProduct(string vs, string evs, double expected)
        {
            var v1 = Vector2.Parse(vs);
            var v2 = Vector2.Parse(evs);
            Assert.AreEqual(expected, v1.DotProduct(v2));
        }

        [TestCase("2, 3", "0.55470019, 0.83205029")]
        public void Normalize(string vs, string evs)
        {
            var v1 = Vector2.Parse(vs);
            var expected = Vector2.Parse(evs);
            Assert.True(expected.Equals(v1.Normalize(), 1e-6));
        }

        [TestCase("1, 1", 1)]
        [TestCase("10, 0", 0)] // Horizontal slope = 0
        [TestCase("0, 10", 0)] // Vertical slope = 0
        [TestCase("3, 4", 1.33333333333333333333333333333)]
        public void Slope(string vs, double expected)
        {
            var v1 = Vector2.Parse(vs);
            Assert.AreEqual(v1.Slope, expected);
        }



        
    }
}
