using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    class Section
    {
        /// <summary>
        /// Координаты секции
        /// </summary>
        protected Coordinates Point;

        /// <summary>
        /// Длина секции
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Геометрическая длина секции
        /// </summary>
        public int RealLength { get; set; }

        /// <summary>
        /// Глубина секции
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Геометрическая глубина секции
        /// </summary>
        public int RealWidth { get; set; }

        /// <summary>
        /// Координаты секции
        /// </summary>
        public Coordinates XY
        {
            get
            { return Point; }
            set
            { Point = value; }
        }

        /// <summary>
        /// Изменение точки вставки
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void setPoint(int X, int Y)
        {
            Point.XY = new Point(X, Y);
            Point.BottomLeft = Point.XY;
            Point.BottomRight = new Point(X + RealWidth, Y);
            Point.TopLeft = new Point(X, Y + RealLength);
            Point.TopRight = new Point(X + RealWidth, Y + RealLength);

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Length">Длина секции</param>
        /// <param name="Width">Глубина секции</param>
        /// <param name="insPoint">Точка вставки</param>
        public Section(int Length, int Width, Point insPoint)
        {
            Point = new Coordinates();

            this.Length = Length;
            this.Width = Width;
            this.RealLength = Length;
            this.RealWidth = Width;
            setPoint(insPoint.X, insPoint.Y);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Length">Длина секции базовая</param>
        /// <param name="Width">Глубина секции базовая</param>
        /// <param name="RealLength">Длина секции реальная</param>
        /// <param name="RealWidth">Глубина секции реальная</param>
        /// <param name="X">X координаты</param>
        /// <param name="Y">Y координаты</param>
        public Section(int Length, int Width, int RealLength, int RealWidth, int X, int Y)
        {
            Point = new Coordinates();
            this.Length = Length;
            this.Width = Width;
            this.RealLength = RealLength;
            this.RealWidth = RealWidth;
            setPoint(X, Y);
        }


    }
}
