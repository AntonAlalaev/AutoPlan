using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AutoPlan.Tests
{
    [TestClass]
    public class RectangleTest
    {
        /// <summary>
        /// Проверка на стандартное создание прямоугольника
        /// </summary>
        [TestMethod]
        public void RectangleSquareStandart()
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

        /// <summary>
        /// Проверка на создание прямоугольника по точкам в обратной последовательности
        /// </summary>
        [TestMethod]
        public void RectangleSquareNegative()
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

        /// <summary>
        /// Проверка на создание прямоугольника по инвертированным точкам
        /// </summary>
        [TestMethod]
        public void RectangleNegativeBasePoint1()
        {
            // arrange
            Point One = new Point(20, 23);
            Point Two = new Point(110, -74);

            Point expectedTopRight = new Point(110, 23);
            Point expectedBottomLeft = new Point(20, -74);

            int Height = 97;
            int Length = 90;

            int ExpectedSquare = Height * Length;
            int ExpectedPerimetr = Height * 2 + Length * 2;
            // act
            Rectangle rec1 = new Rectangle(One, Two);

            // assert
            Assert.AreEqual(rec1.TopRight, expectedTopRight);
            Assert.AreEqual(rec1.BottomLeft, expectedBottomLeft);
            Assert.AreEqual(rec1.TopLeft, One);
            Assert.AreEqual(rec1.BottomRight, Two);
            Assert.AreEqual(rec1.Square, ExpectedSquare);
            Assert.AreEqual(rec1.Perimetr, ExpectedPerimetr);
            Assert.AreEqual(rec1.Length, Length);
            Assert.AreEqual(rec1.Height, Height);
        }

        /// <summary>
        /// Проверка на пересечение прямоугольников
        /// Заданных различными способами
        /// с касанием
        /// с общими границами
        /// с разными границами
        /// </summary>
        [TestMethod]
        public void RectangleIntersectionSingle()
        {
            // arrange
            // act
            Rectangle BigOne = new Rectangle(new Point(20, -74), new Point(110, 23));
            Rectangle SecondBig = new Rectangle(new Point(-57, -6), new Point(-6, -63));
            Rectangle MiddleIntersect = new Rectangle(new Point(-13, -17), new Point(18, -44));
            Rectangle SmallInternal = new Rectangle(new Point(-2, -23), new Point(14, -37));
            Rectangle SmallestOne = new Rectangle(new Point(-5, -45), new Point(7, -59));

            // assert

            // Большой прямоугольник
            Assert.IsFalse(BigOne.IntersectWith(SecondBig));
            Assert.IsFalse(BigOne.IntersectWith(MiddleIntersect));
            Assert.IsFalse(BigOne.IntersectWith(SmallInternal));
            Assert.IsFalse(BigOne.IntersectWith(SmallestOne));

            // Второй большой прямоугольник
            Assert.IsTrue(SecondBig.IntersectWith(MiddleIntersect));
            Assert.IsFalse(SecondBig.IntersectWith(BigOne));
            Assert.IsFalse(SecondBig.IntersectWith(SmallInternal));
            Assert.IsFalse(SecondBig.IntersectWith(SmallestOne));

            // Средний прямоугольник
            Assert.IsTrue(MiddleIntersect.IntersectWith(SecondBig));
            Assert.IsTrue(MiddleIntersect.IntersectWith(SmallInternal));
            Assert.IsFalse(MiddleIntersect.IntersectWith(BigOne));
            Assert.IsFalse(MiddleIntersect.IntersectWith(SmallestOne));

            // Маленький внутренний прямоугольник
            Assert.IsFalse(SmallInternal.IntersectWith(BigOne));
            Assert.IsFalse(SmallInternal.IntersectWith(SecondBig));
            Assert.IsFalse(SmallInternal.IntersectWith(SmallestOne));
            Assert.IsTrue(SmallInternal.IntersectWith(MiddleIntersect));

            // Маленький прямоугольник
            Assert.IsFalse(SmallestOne.IntersectWith(BigOne));
            Assert.IsFalse(SmallestOne.IntersectWith(SecondBig));
            Assert.IsFalse(SmallestOne.IntersectWith(MiddleIntersect));
            Assert.IsFalse(SmallestOne.IntersectWith(SmallInternal));

            // Проверка прямоугольников по границам
            Assert.IsTrue(BigOne.IntersectWith(MiddleIntersect, 3));
            Assert.IsTrue(BigOne.IntersectWith(MiddleIntersect, 2));
            Assert.IsFalse(BigOne.IntersectWith(MiddleIntersect, 1));
        }

        [TestMethod]
        public void TestSection()
        {
            // arrange
            string FileName = "LoadedSections.xml";
            List<Section> test = Parametrs.LoadSection(FileName);
            double testLength = 0;
            bool testMain = true;
            foreach (Section Item in test)
            {
                if (Item.Name == "ПО 2065х1250х300")
                {
                    testLength = Item.FakeLength;
                }
                if (Item.Name == "ПД 2065х1000х300")
                {
                    testMain = Item.Main;
                }
            }

            // assert
            Assert.IsTrue(test.Count > 0);
            Assert.IsTrue(testLength == 1250);
            Assert.IsFalse(testMain);
        }

        [TestMethod]
        public void TestRotation()
        {
            // arrange

            Rectangle t1 = new Rectangle(new Point(4, 4), new Point(9, 15));
            Rectangle p1 = new Rectangle(new Point(27, -3), new Point(32, 8));

            // act
            Rectangle t2 = Calculation.TransformForward(t1, Calculation.Transform.Left);
            Rectangle p2 = Calculation.TransformForward(p1, Calculation.Transform.Top);

            // assert
            // нижняя точка прямоугольника t2
            Assert.IsTrue(Math.Abs(t2.BottomLeft.X + 15) < 0.1);
            Assert.IsTrue(Math.Abs(t2.BottomLeft.Y - 4) < 0.1);
        }
    }
}