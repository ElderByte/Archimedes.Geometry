using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Geometry.Primitives;
using Archimedes.Geometry.Units;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class Rectangle2Tests
    {
        [TestCase("100, 100", "500, 300", "0°")]
        [TestCase("0, 0", "50, 50", "0°")]
        public void Constructor(string locationStr, string sizeStr, string rotationStr)
        {
            var location = Vector2.Parse(locationStr);
            var size = SizeD.Parse(sizeStr);
            var rotation = Angle.Parse(rotationStr);

            var rect = new Rectangle2(location, size, rotation);

            Assert.AreEqual(location, rect.Location);
            Assert.AreEqual(size.Width, rect.Width);
            Assert.AreEqual(size.Height, rect.Height);
            Assert.AreEqual(rotation, rect.Rotation);
        }

        [TestCase("(0,0),(100,0),(120,120), (0,100)")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorFail(string rectStr)
        {
            var vertices = Vector2.ParseAll(rectStr);
            var rect = new Rectangle2(vertices);
        }

        [TestCase("100, 100", "500, 300", "0°")]
        [TestCase("100, 100", "500, 300", "45°")]
        public void LocationRotation(string locationStr, string sizeStr, string rotationStr)
        {
            var location = Vector2.Parse(locationStr);
            var size = SizeD.Parse(sizeStr);
            var rotation = Angle.Parse(rotationStr);

            var rect = new Rectangle2(location, size, rotation);

            Assert.AreEqual(location, rect.Location);
            Assert.AreEqual(size.Width, rect.Width, GeometrySettings.DEFAULT_TOLERANCE);
            Assert.AreEqual(size.Height, rect.Height, GeometrySettings.DEFAULT_TOLERANCE);
            Assert.AreEqual(rotation, rect.Rotation);
        }

        [TestCase("(0, 0), (100, 0), (100, 200), (0, 200)", "50, 100", "0°")]
        //[TestCase("(0, 0), (100, 0), (100, 200), (0, 200)", "-100, 50", "90°")]
        // TODO add more tests with rotation
        public void FromVertices(string verticesStr, string expectedMiddleStr, string expectedRotationStr)
        {
            var vertices = Vector2.ParseAll(verticesStr);
            var expectedMiddle = Vector2.Parse(expectedMiddleStr);


            var rect = new Rectangle2(vertices);
            var expectedRotation = Angle.Parse(expectedRotationStr);

            Assert.AreEqual(vertices[0], rect.Location);
            Assert.AreEqual(expectedMiddle, rect.MiddlePoint);
            Assert.AreEqual(expectedRotation, rect.Rotation);
        }

        
        [TestCase("(0,0),(0,0),(0,0),(0,0)", 200, 100)]             // Specail case: Empty rect!
        [TestCase("(0,0),(200,0),(200,100), (0,100)", 200, 100)]    // Same size
        [TestCase("(0,0),(10,0),(10,45), (0,45)", 200, 100)]        // Different origin size
        public void Constructor2(string rectStr, double witdh, double height)
        {
            var vertices = Vector2.ParseAll(rectStr);
            var rect = new Rectangle2(vertices);

            rect.Width = witdh;
            rect.Height = height;

            var location = rect.Location;

            Assert.AreEqual(vertices[0], location);         // Location must stay the same
            Assert.AreEqual(witdh, rect.Width);             // width must be as set
            Assert.AreEqual(height, rect.Height);           // height must be as set
            Assert.AreEqual(Angle.Zero, rect.Rotation);     // Rotation must be Zero
        }


        [TestCase]
        public void EmptyTests()
        {
            var rect = Rectangle2.Empty;

            // Check the empty rect
            Assert.AreEqual(Vector2.Zero, rect.Location);
            Assert.AreEqual(0, rect.Width);
            Assert.AreEqual(0, rect.Height);
            Assert.AreEqual(Angle.Zero, rect.Rotation);     

            // Check how changes to the empty behave

            rect.Location = new Vector2(100, 100);
            // Check the empty rect
            Assert.AreEqual(new Vector2(100, 100), rect.Location);
            Assert.AreEqual(0, rect.Width);
            Assert.AreEqual(0, rect.Height);
            Assert.AreEqual(Angle.Zero, rect.Rotation);


            rect = Rectangle2.Empty;
            rect.Width = 100;
            rect.Height = 50;

            Assert.AreEqual(Vector2.Zero, rect.Location);
            Assert.AreEqual(100, rect.Width);
            Assert.AreEqual(50, rect.Height);
            Assert.AreEqual(Angle.Zero, rect.Rotation);   
        }

        [TestCase("(0,0),(100,0),(100,100),(0,100)", "(100,100)")] 
        public void Middlepoint(string rectStr, string newMiddleStr)
        {
            var vertices = Vector2.ParseAll(rectStr);
            var rect = new Rectangle2(vertices);

            var newMid = Vector2.Parse(newMiddleStr);

            rect.MiddlePoint = newMid;


            Assert.AreEqual(newMid, rect.MiddlePoint);
        }

        [TestCase("(0,0),(0,0),(0,0),(0,0)", "(100,100)")]             // Specail case: Empty rect!
        [ExpectedException(typeof(NotSupportedException))]
        public void MiddlepointException(string rectStr, string newMiddleStr)
        {
            var vertices = Vector2.ParseAll(rectStr);
            var rect = new Rectangle2(vertices);

            var newMid = Vector2.Parse(newMiddleStr);

            rect.MiddlePoint = newMid;


            Assert.AreEqual(newMid, rect.MiddlePoint);
        }

        /*
         * TODO Cant fix this right now...
        [TestCase]
        public void NegativeLocation()
        {
            var rect = new Rectangle2(new Vector2(-6.144577234345E+26, 2.57349556991535E+28), SizeD.Empty);
            rect.Width = 37;
            rect.Height = 15;
            Assert.AreEqual(100, rect.Width);
            Assert.AreEqual(200, rect.Height);
        }*/


    }
}
