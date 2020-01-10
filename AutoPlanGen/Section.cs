﻿using System;
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
        public Section(string Name, int RealLength, int RealWidth, int FormalLength, int FormalWidth, int SectionHeight, bool Main = false) : base(new Point(0, 0), RealWidth, RealLength)
        {
            this.Name = Name;
            this.Main = Main;
            FakeLength = FormalLength;
            FakeWidth = FormalWidth;
            SecHeight = SectionHeight;
            
        }

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
        /// Основная секция
        /// </summary>
        public bool Main { get; set; }



    }
}
