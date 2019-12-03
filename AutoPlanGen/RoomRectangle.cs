using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    public class RoomRectangle : Rectangle
    {
        /// <summary>
        /// Базовый конструктор по двум точкам
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="TopRight"></param>
        public RoomRectangle(Point BottomLeft, Point TopRight) : base(BottomLeft, TopRight)
        {
            Initialaize();
        }

        /// <summary>
        /// Инициализатор класса
        /// </summary>
        private void Initialaize()
        {
            Obstacles = new List<Rectangle>();
        }

        /// <summary>
        /// Препядствия в виде прямоугольников
        /// </summary>
        protected List<Rectangle> Obstacles;

        /// <summary>
        /// Добавление препядствий в помещение
        /// </summary>
        /// <param name="Obstacles"></param>
        public void AddObstacles(List<Rectangle> Obstacles)
        {
            List<Rectangle> tmp = IntersectWith(Obstacles);
            if (tmp.Count != 0)
                this.Obstacles.AddRange(tmp);
        }

        /// <summary>
        /// Добавление препядствия в помещение
        /// </summary>
        /// <param name="Obstacle">Препядствие</param>
        public void AddObstacle(Rectangle Obstacle)
        {
            if (IntersectWith(Obstacle))
                Obstacles.Add(Obstacle);
        }

        /// <summary>
        /// Проверка на дублирующиеся препядствия
        /// </summary>
        private void CheckDoubleObstacles()
        {
            List<Rectangle> result = Obstacles.Distinct().ToList();
            Obstacles = result;
            //Dictionary<int, Rectangle> Dic = new Dictionary<int, Rectangle>();
            //int Index = 0;
            //foreach (Rectangle Item in Obstacles)
            //{
            //    if (!Dic.ContainsValue(Item))
            //    {
            //        Dic.Add(Index, Item);
            //        Index++;
            //    }
            //}
            //Obstacles = Dic.Values.ToList();
        }

    }
}
