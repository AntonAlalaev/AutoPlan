using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    public class Section : Rectangle
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
        /// Конструктор для полноценной секции
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="RealLength"></param>
        /// <param name="RealWidth"></param>
        /// <param name="FormalLength"></param>
        /// <param name="FormalWidth"></param>
        /// <param name="SectionHeight"></param>
        public Section(string Name, int RealLength, int RealWidth, int FormalLength, int FormalWidth, int SectionHeight, bool Double, bool Main, bool Stationary) : base(new Point(0, 0), RealWidth, RealLength)
        {
            this.Name = Name;
            this.Main = Main;
            this.Double = Double;
            this.Stationary = Stationary;
            FakeLength = FormalLength;
            FakeWidth = FormalWidth;
            SecHeight = SectionHeight;            
        }

        /// <summary>
        /// Конструктор на базе существующей секции
        /// </summary>
        /// <param name="BaseSection">Исходная секция</param>
        /// <param name="BottomLeft">Нижняя левая точка привязки новой секции</param>
        public Section(Section BaseSection, Point BottomLeft): base (BottomLeft, BaseSection.Length, BaseSection.Height)
        {
            Name = BaseSection.Name;
            Main = BaseSection.Main;
            Double = BaseSection.Double;
            FakeLength = BaseSection.Length;
            FakeWidth = BaseSection.FakeWidth;
            SecHeight = BaseSection.SecHeight;
            Stationary = BaseSection.Stationary;
        }

        /// <summary>
        /// Признак двухсторонней секции
        /// </summary>
        public bool Double { get; set; }

        /// <summary>
        /// Наименование секции
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Геометрическая длина секции
        /// </summary>
        public double FakeLength { get; set; }


        /// <summary>
        /// Геометрическая глубина секции
        /// </summary>
        public double FakeWidth { get; set; }
        
        /// <summary>
        /// Высота секции
        /// </summary>
        public int SecHeight { get; set; }

        /// <summary>
        /// Признак стационарной секции
        /// </summary>
        public bool Stationary { get; set; }

        /// <summary>
        /// Основная секция
        /// </summary>
        public bool Main { get; set; }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + " (" + BottomLeft.ToString() + ")";
        }



    }
}
