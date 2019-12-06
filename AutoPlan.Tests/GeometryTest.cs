using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoPlan.Tests
{
    [TestClass]
    public class GeometryTest
    {
        [TestMethod]
        public void CrossProductLength()
        {
            // arrange
            Point One = new Point(1109, 2252); // a
            Point Two = new Point(1109, 1168); // b
            Point Three = new Point(2008, 1168); // c

            // act

            double Vector = Geometry.CrossProductLength(One, Two, Three);
            double Angle = Geometry.getAngle(One, Two, Three);
            // assert
            Assert.IsTrue(Math.Abs(Angle + Math.PI/2) < 0.1);
            Assert.IsTrue(Vector < 0);

        }
    }
}
