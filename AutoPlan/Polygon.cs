using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
