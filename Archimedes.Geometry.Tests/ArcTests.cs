using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archimedes.Geometry.Units;
using Archimedes.Geometry.Primitives;
using NUnit.Framework;

namespace Archimedes.Geometry.Tests
{
    public class ArcTests
    {
        [TestCase(177.19600347176961, "237.864°", "152.23617553710937, -90.678390502929688", "329,220159547891°")]
        public void ArcAngleToX(double radius, string arcAngle, string baseVector,  string expected)
        {
            var arc = new Arc(radius, Angle.Parse(arcAngle), Vector2.Parse(baseVector))
            {
                Location = new Vector2(140, 140),
            };
            var expectedAngle = Angle.Parse(expected);
            Assert.AreEqual(expectedAngle, arc.Angle2X);
        }
    }
}
