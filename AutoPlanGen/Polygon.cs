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
        /// Список вершин прямоугольника
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

                Point pij1 = new Point((double)(old_points[i].X + n1.X), (double)(old_points[i].Y + n1.Y));
                Point pij2 = new Point((double)(old_points[j].X + n1.X), (double)(old_points[j].Y + n1.Y));

                Vector v2 = new Vector(old_points[k].X - old_points[j].X, old_points[k].Y - old_points[j].Y);
                v2.Normalize();
                v2 *= offset;
                Vector n2 = new Vector(-v2.Y, v2.X);

                Point pjk1 = new Point((double)(old_points[j].X + n2.X), (double)(old_points[j].Y + n2.Y));
                Point pjk2 = new Point((double)(old_points[k].X + n2.X), (double)(old_points[k].Y + n2.Y));

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect, segments_intersect;
                Point poi, close1, close2;
                FindIntersection(pij1, pij2, pjk1, pjk2, out lines_intersect, out segments_intersect, out poi, out close1, out close2);
                Debug.Assert(lines_intersect, "Edges " + i + "-->" + j + " and " + j + "-->" + k + " are parallel");
                enlarged_points.Add(poi);
            }
            return enlarged_points;
        }


        /// <summary>
        /// Находит точку пересечения между длиниями
        /// p1-->p2 и p3-->p4
        /// </summary>
        /// <param name="p1">Первая точка первой линии</param>
        /// <param name="p2">Вторая точка первой линии</param>
        /// <param name="p3">Первая точка второй линии</param>
        /// <param name="p4">Вторая точка второй линии</param>
        /// <param name="lines_intersect">Пересекаются ли линии?</param>
        /// <param name="segments_intersect">Пересекаются ли заданные сегменеты</param>
        /// <param name="intersection">точка пересечения</param>
        /// <param name="close_p1">Ближайшая точка на линии 1</param>
        /// <param name="close_p2">Ближайшая точка на линии 2</param>
        private static void FindIntersection(Point p1, Point p2, Point p3, Point p4, out bool lines_intersect, out bool segments_intersect, out Point intersection, out Point close_p1, out Point close_p2)
        {
            // Get the segments' parameters.
            double dx12 = p2.X - p1.X;
            double dy12 = p2.Y - p1.Y;
            double dx34 = p4.X - p3.X;
            double dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point(double.NaN, double.NaN);
                close_p1 = new Point(double.NaN, double.NaN);
                close_p2 = new Point(double.NaN, double.NaN);
                return;
            }
            lines_intersect = true;

            double t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // Find the point of intersection.
            intersection = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }
            close_p1 = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }


    }
}
