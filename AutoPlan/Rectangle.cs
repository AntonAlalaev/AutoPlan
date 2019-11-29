using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    public class Rectangle
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
        /// Базовый конструктор 2 точки
        /// </summary>
        /// <param name="BottomLeft">Нижний левый угол</param>
        /// <param name="TopRight">Верхний правый угол</param>
        public Rectangle(Point BottomLeft, Point TopRight)
        {
            // необходимо сначала понять где самые крайние точки            
            // минимальная по X
            int MinX;
            if (BottomLeft.X < TopRight.X)
                MinX = BottomLeft.X;
            else
                MinX = TopRight.X;

            // минимальная по Y
            int MinY;
            if (BottomLeft.Y < TopRight.Y)
                MinY = BottomLeft.Y;
            else
                MinY = TopRight.Y;

            // максимальная по Х
            int MaxX;
            if (BottomLeft.X > TopRight.X)
                MaxX = BottomLeft.X;
            else
                MaxX = TopRight.X;

            // максимальная по Y
            int MaxY;
            if (BottomLeft.Y > TopRight.Y)
                MaxY = BottomLeft.Y;
            else
                MaxY = TopRight.Y;

            this.BottomLeft = new Point(MinX, MinY);
            this.TopRight = new Point(MaxX, MaxY);

            BottomRight = new Point(this.TopRight.X, this.BottomLeft.Y);
            TopLeft = new Point(this.BottomLeft.X, this.TopRight.Y);
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
        /// Периметр прямоугольника
        /// </summary>
        public int Perimetr
        { get { return Length * 2 + Height * 2; } }

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

        /// <summary>
        /// проверка на идентичность элементов
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        private static bool Equals(Rectangle obj1, Rectangle obj2)
        {
            if (obj1.BottomLeft == obj2.BottomLeft && obj1.BottomRight == obj2.BottomRight && obj1.TopLeft == obj2.TopLeft && obj1.TopRight == obj2.TopRight)
                return true;
            return false;
        }
        /// <summary>
        /// Сравнение
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==(Rectangle obj1, Rectangle obj2)
        {
            if (Equals(obj1, obj2))
                return true;
            return false;
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=(Rectangle obj1, Rectangle obj2)
        {
            if (Equals(obj1, obj2))
                return false;
            return true;
        }

        /// <summary>
        /// Переопределение сравнителя
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as Rectangle);
        }

        /// <summary>
        /// Переопределение Хэш-кода
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 352033281;
            hashCode = hashCode * -1521134295 + TopLeft.GetHashCode();
            hashCode = hashCode * -1521134295 + TopRight.GetHashCode();
            hashCode = hashCode * -1521134295 + BottomLeft.GetHashCode();
            hashCode = hashCode * -1521134295 + BottomRight.GetHashCode();
            return hashCode;
        }

    }
}
