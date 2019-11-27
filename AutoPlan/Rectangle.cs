﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    class Rectangle
    {
        /// <summary>
        /// Координаты нижнего левого угла
        /// </summary>
        public Point BottomLeft { get; set; }

        /// <summary>
        /// Координаты нижнего правого угла
        /// </summary>
        public Point BottomRight { get; set; }

        /// <summary>
        /// Координаты верхнего левого угла
        /// </summary>
        public Point TopLeft { get; set; }

        /// <summary>
        /// Координаты верхнего правого угла
        /// </summary>
        public Point TopRight { get; set; }

        /// <summary>
        /// Базовый конструктор 4 точки
        /// </summary>
        /// <param name="BottomLeft">Нижний левый угол</param>
        /// <param name="BottomRight">Нижний правый угол</param>
        /// <param name="TopLeft">Верхний левый угол</param>
        /// <param name="TopRight">Верхний правый угол</param>
        public Rectangle(Point BottomLeft, Point BottomRight, Point TopLeft, Point TopRight)
        {
            this.BottomLeft = BottomLeft;
            this.BottomRight = BottomRight;
            this.TopLeft = TopLeft;
            this.TopRight = TopRight;
        }

        /// <summary>
        /// Базовый конструктор 2 точки
        /// </summary>
        /// <param name="BottomLeft">Нижний левый угол</param>
        /// <param name="TopRight">Верхний правый угол</param>
        public Rectangle(Point BottomLeft, Point TopRight)
        {
            this.BottomLeft = BottomLeft;
            BottomRight = new Point(TopRight.X, BottomLeft.Y);
            this.TopRight = TopRight;
            TopLeft = new Point(BottomLeft.X, TopRight.Y);
        }

        /// <summary>
        /// Базовый конструктор 1 точка + длина и высота
        /// </summary>
        /// <param name="BottomLeft">Нижний левый угол</param>
        /// <param name="Length">Длина</param>
        /// <param name="Height">Высота</param>
        public Rectangle(Point BottomLeft, int Length, int Height)
        {
            this.BottomLeft = BottomLeft;
            BottomRight = new Point(BottomLeft.X + Length, BottomLeft.Y);
            TopRight = new Point(BottomLeft.X + Length, BottomLeft.Y + Height);
            TopLeft = new Point(BottomLeft.X, BottomLeft.Y + Height);
        }

        /// <summary>
        /// Длина прямоугольника
        /// </summary>
        public int Length
        { get { return TopRight.X - BottomLeft.X; } }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public int Height
        { get { return TopRight.Y - BottomLeft.Y; } }

        /// <summary>
        /// Площадь прямоугольника
        /// </summary>
        public int Square
        { get { return Length * Height; } }

        /// <summary>
        /// Пересекает ли текущий прямоугольник заданный
        /// </summary>
        /// <param name="obj">заданный прямоугольник</param>
        /// <returns></returns>
        public bool IntersectWith(Rectangle obj)
        {
            // проверяем пересечение
            if ((obj.TopRight.X >= this.BottomLeft.X && obj.BottomLeft.X <= this.TopRight.X) &&
                (obj.TopRight.Y >= this.BottomLeft.Y && obj.BottomLeft.Y <= this.TopRight.Y))
                return true;
            return false;
        }

        /// <summary>
        /// Пересекает ли текущий прямоугольник заданный
        /// с указанной границей общей
        /// </summary>
        /// <param name="obj">заданный прямоугольник</param>
        /// <param name="Border">граница добавочная</param>
        /// <returns></returns>
        public bool IntersectWith(Rectangle obj, int Border)
        {
            Rectangle modified = new Rectangle(new Point(obj.BottomLeft.X - Border, obj.BottomLeft.Y - Border), new Point(obj.TopRight.X + Border, obj.TopRight.Y + Border));
            return IntersectWith(modified);
        }

        /// <summary>
        /// Пересекает ли текущий прямоугольник заданный
        /// с заданными границами
        /// </summary>
        /// <param name="obj">заданный прямоугольник</param>
        /// <param name="Top">верхняя граница</param>
        /// <param name="Bottom">нижняя граница</param>
        /// <param name="Left">левая граница</param>
        /// <param name="Right">правая граница</param>
        /// <returns></returns>
        public bool IntersectWith(Rectangle obj, int Top, int Bottom, int Left, int Right)
        {
            Rectangle modified = new Rectangle(new Point(obj.BottomLeft.X - Left, obj.BottomLeft.Y - Bottom), new Point(obj.TopRight.X + Right, obj.TopRight.Y + Top));
            return IntersectWith(modified);
        }


        /// <summary>
        /// Сортировка списка прямоугольников по координатам
        /// </summary>
        /// <param name="RectList">Список с прямоугольниками</param>
        /// <param name="X">Сортировать по X - true или Y - false </param>
        /// <returns></returns>
        protected static List<Rectangle> SortBy(List<Rectangle> RectList, bool X = true)
        {
            if (X)
                return RectList.OrderBy(x => x.BottomLeft.X).ToList();
            else
                return RectList.OrderBy(x => x.BottomLeft.Y).ToList();
        }

        /// <summary>
        /// Сортировка списка прямоугольников по X
        /// </summary>
        /// <param name="RectList">Список с прямоугольниками</param>
        /// <returns></returns>
        public static List<Rectangle> SortByX(List<Rectangle> RectList)
        {
            return SortBy(RectList);
        }

        /// <summary>
        /// Сортировка списка прямоугольников по Y
        /// </summary>
        /// <param name="RectList">Список с прямоугольниками</param>
        /// <returns></returns>
        public static List<Rectangle> SortByY(List<Rectangle> RectList)
        {
            return SortBy(RectList, false);
        }

        /// <summary>
        /// Возвращает список прямоугольников с которыми пересекается (граничит)
        /// заданный прямоугольник
        /// </summary>
        /// <param name="Element">Заданный прямоугольник</param>
        /// <param name="RectList">Список прямоугольников</param>
        /// <returns></returns>
        public static List<Rectangle> IntersectWith(Rectangle Element, List<Rectangle> RectList)
        {
            List<Rectangle> ret = new List<Rectangle>();
            foreach (Rectangle Item in RectList)
                if (Element.IntersectWith(Item))
                    ret.Add(Item);
            return ret;
        }

        /// <summary>
        /// Возвращает список прямоугольников с которыми пересекается (граничит)
        /// текущий прямоугольник
        /// </summary>
        /// <param name="RectList">Список прямоугольников</param>
        /// <returns></returns>
        public List<Rectangle> IntersectWith(List<Rectangle> RectList)
        {
            return IntersectWith(this, RectList);
        }

        /// <summary>
        /// Создает прямоугольник по границами точек в списке
        /// </summary>
        /// <param name="Points">Спиоск точек</param>
        /// <returns></returns>
        public static Rectangle RectangleByListPoint(List<Point> Points)
        {
            // сначала находим самую минимальную точку по X, Y
            int MinX = Points.Min(n => n.X);
            int MinY = Points.Min(n => n.Y);

            // затем максимальную по X,Y
            int MaxX = Points.Max(n => n.X);
            int MaxY = Points.Max(n => n.Y);


            // создаем прямоугольник для возврата по заданным граничным координатам
            return new Rectangle(new Point(MinX, MinY), new Point(MaxX, MaxY));
        }


    }
}
