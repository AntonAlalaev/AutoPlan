using System;
using System.Windows;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AutoPlan
{
    /// <summary>
    /// Класс многоукольник
    /// </summary>
    public class Polygon
    {
        /// <summary>
        /// Список вершин многоугольника
        /// </summary>
        protected List<Point> VertexList;

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public Polygon(List<Point> Vertices)
        {
            VertexList = Vertices;
        }

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public Polygon()
        {
            VertexList = new List<Point>();
        }

        /// <summary>
        /// Добавление вершины
        /// </summary>
        /// <param name="Vertex"></param>
        public void AddVertex(Point Vertex)
        {
            VertexList.Add(Vertex);
        }

        /// <summary>
        /// Добавление списка вершин
        /// </summary>
        /// <param name="Vertices"></param>
        public void AddVertices(List<Point> Vertices)
        {
            VertexList.AddRange(Vertices);
        }

        /// <summary>
        /// Находится ли точка внутри многоугольника
        /// </summary>
        /// <param name="PointIn">Искомая точка</param>
        /// <returns></returns>
        public bool isPointIn(Point PointIn)
        {
            if (VertexList.Count < 3)
                return false;
            bool result = false;
            int size = VertexList.Count;
            int j = size - 1;
            for (int i = 0; i < size; i++)
            {
                if ((VertexList[i].Y < PointIn.Y && VertexList[j].Y >= PointIn.Y ||
                     VertexList[j].Y < PointIn.Y && VertexList[i].Y >= PointIn.Y) &&
                     (VertexList[i].X + (PointIn.Y - VertexList[i].Y) /
                     (VertexList[j].Y - VertexList[i].Y) * (VertexList[j].X - VertexList[i].X) < PointIn.X))
                    result = !result;
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Вершины многоугольнинка
        /// </summary>
        public List<Point> Vertices
        {
            get { return VertexList; }
        }

        /// <summary>
        /// Пересекает ли текущий полигон заданный
        /// </summary>
        /// <param name="obj">заданный полигон</param>
        /// <returns></returns>
        public bool isIntersect(Polygon obj)
        {
            foreach (Point Item in obj.VertexList)
                if (isPointIn(Item))
                    return true;
            return false;
        }

        /// <summary>
        /// Пересекает ли текущий полигон заданный прямоугольник
        /// </summary>
        /// <param name="obj">заданный прямоугольник</param>
        /// <returns></returns>
        public bool isIntersect(Rectangle obj)
        {
            if (isPointIn(obj.BottomLeft) || isPointIn(obj.BottomRight) || isPointIn(obj.TopRight) || isPointIn(obj.TopLeft))
                return true;
            return false;
        }

        /// <summary>
        /// Выпуклый ли полигон
        /// </summary>
        public bool isConvex
        {
            get
            {
                bool got_negative = false;
                bool got_positive = false;
                int num_points = VertexList.Count;
                int B, C;
                for (int A = 0; A < num_points; A++)
                {
                    B = (A + 1) % num_points;
                    C = (B + 1) % num_points;
                    double cross_product = Geometry.CrossProductLength(VertexList[A], VertexList[B], VertexList[C]);
                    if (cross_product < 0)
                    {
                        got_negative = true;
                    }
                    else if (cross_product > 0)
                    {
                        got_positive = true;
                    }
                    if (got_negative && got_positive) return false;
                }
                // If we got this far, the polygon is convex.
                return true;
            }
        }

        /// <summary>
        /// Площадь многоугольника 
        /// </summary>
        public double Area
        {
            get
            {
                if (VertexList.Count < 3)
                    return 0;
                double result = 0;
                int j = VertexList.Count - 1;
                for (int i = 0; i < VertexList.Count; i++)
                {
                    result += (VertexList[j].X + VertexList[i].X) * (VertexList[j].Y - VertexList[i].Y);
                    j = i;
                }
                return result / 2;
            }
        }


        /// <summary>
        /// Возвращает описанный прямоугольник вокруг заданного многоугольника
        /// </summary>
        /// <param name="Source">Заданный многоугольник</param>
        /// <returns></returns>
        public static Rectangle getOuterRectangle(Polygon Source)
        {
            double MinX = Source.VertexList.Min(n => n.X);
            double MaxX = Source.VertexList.Max(n => n.X);
            double MinY = Source.VertexList.Min(n => n.Y);
            double MaxY = Source.VertexList.Max(n => n.Y);
            Rectangle result = new Rectangle(new Point(MinX, MinY), new Point(MaxX, MaxY));
            return result;
        }

        /// <summary>
        /// Возвращает описанный прямоугольник
        /// </summary>
        public Rectangle OuterRectangle
        {
            get
            { return getOuterRectangle(this); }
        }

        /// <summary>
        /// Сдвиг полигона
        /// </summary>
        /// <param name="IncX">смещение по X</param>
        /// <param name="IncY">смещение по Y</param>
        public void Move(double IncX, double IncY)
        {
            for (int i = 0; i < VertexList.Count; i++)
            {
                VertexList[i].X += IncX;
                VertexList[i].Y += IncY;
            }
        }

        /// <summary>
        /// Возвращает полигон увеличенный на заданное смещение
        /// </summary>
        /// <param name="Source">Исходный полигон</param>
        /// <param name="Offset">Смещение</param>
        /// <returns></returns>
        public static Polygon GetOffsetPolygon(Polygon Source, double Offset)
        {

            List<Point> old_points = Source.VertexList;
            List<Point> new_points = GetEnlargedPolygon(old_points, Offset);
            Polygon result = new Polygon(new_points);
            return result;

        }

        /// <summary>
        /// Возвращает текущий полигон увеличенный на заданное смещение
        /// </summary>
        /// <param name="Offset">Смещение</param>
        /// <returns></returns>
        public Polygon GetOffsetPolygon(double Offset)
        {
            return GetOffsetPolygon(this, Offset);
        }

        /// <summary>
        /// Возвращает точки полигона увеличенного на соответствующее значение
        /// </summary>
        /// <param name="old_points"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static List<Point> GetEnlargedPolygon(List<Point> old_points, double offset)
        {
            List<Point> enlarged_points = new List<Point>();
            int num_points = old_points.Count;
            for (int j = 0; j < num_points; j++)
            {
                // Find the new location for point j.
                // Find the points before and after j.
                int i = (j - 1);
                if (i < 0)
                    i += num_points;
                int k = (j + 1) % num_points;

                // Move the points by the offset.
                Vector v1 = new Vector(old_points[j].X - old_points[i].X, old_points[j].Y - old_points[i].Y);
                v1.Normalize();
                v1 *= offset;
                Vector n1 = new Vector(-v1.Y, v1.X);

                Point pij1 = new Point(old_points[i].X + n1.X, old_points[i].Y + n1.Y);
                Point pij2 = new Point(old_points[j].X + n1.X, old_points[j].Y + n1.Y);

                Vector v2 = new Vector(old_points[k].X - old_points[j].X, old_points[k].Y - old_points[j].Y);
                v2.Normalize();
                v2 *= offset;
                Vector n2 = new Vector(-v2.Y, v2.X);

                Point pjk1 = new Point(old_points[j].X + n2.X, old_points[j].Y + n2.Y);
                Point pjk2 = new Point(old_points[k].X + n2.X, old_points[k].Y + n2.Y);

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect;
                bool segments_intersect;
                Point poi;
                Point close_p1;
                Point close_p2;
                Geometry.FindIntersection(pij1, pij2, pjk1, pjk2, out lines_intersect, out segments_intersect, out poi, out close_p1, out close_p2);
                Debug.Assert(lines_intersect, "Edges " + i + "-->" + j + " and " + j + "-->" + k + " are parallel");
                enlarged_points.Add(poi);
            }
            return enlarged_points;
        }

        /// <summary>
        /// Возвращает периметр многоугольника
        /// </summary>
        public double Perimetr
        {
            get
            {
                double length = 0;
                for (int i = 0; i < VertexList.Count; i++)
                {
                    if (i == 0)
                        continue;
                    length += Geometry.LineLength(VertexList[i - 1], VertexList[i]);
                }
                if (VertexList.Count > 0)
                    length += Geometry.LineLength(VertexList[VertexList.Count - 1], VertexList[0]);
                return length;
            }
        }

        /// <summary>
        /// Equals objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Polygon Item = (Polygon)obj;
            if (VertexList.Count != Item.VertexList.Count)
                return false;
            for (int i=0; i<VertexList.Count; i++)
            {
                if (VertexList[i] != Item.VertexList[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Hash Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 352030201;
            foreach (Point Item in VertexList)
            {
                hashCode *= -1521104295;
                hashCode += Item.GetHashCode();
            }
            return hashCode;
        }



    }
}
