using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    class Room : Rectangle
    {
        /// <summary>
        /// Базовый конструктор по четырем точкам
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="BottomRight"></param>
        /// <param name="TopLeft"></param>
        /// <param name="TopRight"></param>
        public Room(Point BottomLeft, Point BottomRight, Point TopLeft, Point TopRight) : base(BottomLeft, BottomRight, TopLeft, TopRight)
        {
            Initialaize();
        }

        /// <summary>
        /// Базовый конструктор по двум точкам
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="TopRight"></param>
        public Room(Point BottomLeft, Point TopRight) : base(BottomLeft, TopRight)
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
            this.Obstacles.AddRange(IntersectWith(Obstacles));
        }

    }
}
