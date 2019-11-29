using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoPlan.Tests
{
    [TestClass]
    public class PolygonTest
    {
        [TestMethod]
        public void Polygon_Points_in_bySimple()
        {
            // arrange
            Point One = new Point(0, 0); // no
            Point Two = new Point(3, 2); // yes
            Point Three = new Point(2, 1); // no
            Point Four = new Point(4, 3); // yes
            Point Five = new Point(1, 1); // no

            // act
            Polygon poly1 = new Polygon(new List<Point>() { new Point(1, 0), new Point(0, 1), new Point(1, 1) });
            Polygon poly2 = new Polygon(new List<Point>() { new Point(0, 0), new Point(1, 5), new Point(5, 5), new Point(6, 0) });
            Polygon poly3 = new Polygon(new List<Point>() { new Point(0, 0), new Point(0, 4), new Point(4, 4), new Point(4, 0), new Point(3, 0), new Point(3, 2), new Point(1, 2), new Point(1, 0) });
            Polygon poly4 = new Polygon(new List<Point>() { new Point(0, 0), new Point(0, 4), new Point(4, 4), new Point(4, 0), new Point(3, 0), new Point(3, 2), new Point(1, 2), new Point(1, 0) });
            Polygon poly5 = new Polygon(new List<Point>() { new Point(0, 0), new Point(0, 2), new Point(1, 4), new Point(2, 2), new Point(2, 0), new Point(1, 3) });
            // assert
            Assert.IsFalse(poly1.isPointIn(One));
            Assert.IsTrue(poly2.isPointIn(Two));
            Assert.IsFalse(poly3.isPointIn(Three));
            Assert.IsTrue(poly4.isPointIn(Four));
            Assert.IsFalse(poly5.isPointIn(Five));
        }
    }
}
