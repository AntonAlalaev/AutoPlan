using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    public class Geometry
    {
        /// <summary>
        /// Длина прямой
        /// </summary>
        /// <param name="One">Первая точка</param>
        /// <param name="Two">Вторая точка</param>
        /// <returns></returns>
        public static double LineLength(Point One, Point Two)
        {
            return Math.Sqrt((Two.X - One.X) * (Two.X - One.X) + (Two.Y - One.Y) * (Two.Y - One.Y));
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
        public static void FindIntersection(Point p1, Point p2, Point p3, Point p4, out bool lines_intersect, out bool segments_intersect, out Point intersection, out Point close_p1, out Point close_p2)
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

        /// <summary>
        /// Возвращает длину вектора между линиями заданными тремя точками
        /// </summary>
        /// <param name="A">Точка А</param>
        /// <param name="B">Точка В</param>
        /// <param name="C">Точка С</param>
        /// <returns></returns>
        public static double CrossProductLength(Point A, Point B, Point C)
        {
            return CrossProductLength(A.X, A.Y, B.X, B.Y, C.X, C.Y);
        }

        /// <summary>
        /// Return the cross product AB x BC.
        /// The cross product is a vector perpendicular to AB
        /// and BC having length |AB| * |BC| * Sin(theta) and
        /// with direction given by the right-hand rule.
        /// For two vectors in the X-Y plane, the result is a
        /// vector with X and Y components 0 so the Z component
        /// gives the vector's length and direction.
        /// </summary>
        /// <param name="Ax"></param>
        /// <param name="Ay"></param>
        /// <param name="Bx"></param>
        /// <param name="By"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        public static double CrossProductLength(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        /// <summary>
        /// Return the dot product of AB*BC
        /// Note that AB*BC = |AB|*|BC| *Cos (theta)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double DotProduct(Point A, Point B, Point C)
        {
            // get the vectors components
            double ABx = A.X - B.X;
            double ABy = A.Y - B.Y;
            double BCx = C.X - B.X;
            double BCy = C.Y - B.Y;

            // Calculate the dot product
            return (ABx * BCx + ABy * BCy);
        }

        /// <summary>
        /// Return the Angle between Pi - Pi
        /// Note than the value is the opposite of what you might
        /// Except because Y coodrinates increase downward
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static double getAngle(Point A, Point B, Point C)
        {
            double DotProduct = Geometry.DotProduct(A, B, C);
            double crossProduct = Geometry.CrossProductLength(A, B, C);
            return (double)Math.Atan2(crossProduct, DotProduct);
        }
    }
}
