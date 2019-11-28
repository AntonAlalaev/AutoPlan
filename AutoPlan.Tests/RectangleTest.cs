using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoPlan;

namespace AutoPlan.Tests
{
    [TestClass]
    public class RectangleTest
    {
        [TestMethod]
        public void Rectangle_Square_Standart()
        {
            // arrange
            Point BottomLeft = new Point(0, 0);
            Point TopRight = new Point(100, 50);
            int Expected = 100 * 50;
            // act
            Rectangle rec1 = new Rectangle(BottomLeft, TopRight);
            // assert
            Assert.AreEqual(Expected, rec1.Square);
        }

        [TestMethod]
        public void Rectangle_Square_Negative()
        {
            // arrange
            Point BottomLeft = new Point(100, -50);
            Point TopRight = new Point(0, 0);
            int Expected = 100 * 50;
            // act
            Rectangle rec1 = new Rectangle(BottomLeft, TopRight);
            // assert
            Assert.AreEqual(Expected, rec1.Square);
        }




    }
}
