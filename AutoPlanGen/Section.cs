using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    class Section : Rectangle
    {
        /// <summary>
        /// Конструктор по двум точкам
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="TopRight"></param>
        public Section(Point BottomLeft, Point TopRight) : base(BottomLeft, TopRight)
        {
        }


        /// <summary>
        /// Добавление секции по точке вставки, 
        /// глубине и длине
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public Section(Point BottomLeft, double Width, double Height) : base(BottomLeft, Width, Height)
        {

        }


        /// <summary>
        /// Геометрическая длина секции
        /// </summary>
        public double FakeLength { get; set; }


        /// <summary>
        /// Геометрическая глубина секции
        /// </summary>
        public double FakeWidth { get; set; }


        /// <summary>
        /// Изменение точки вставки
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void setPoint(double X, double Y)
        {

        }


    }
}
