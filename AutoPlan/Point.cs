using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    /// <summary>
    /// Точка геометрическая
    /// </summary>
    class Point
    {
        /// <summary>
        /// Координата X
        /// </summary>
        public int X;

        /// <summary>
        /// Координата Y
        /// </summary>
        public int Y;

        /// <summary>
        /// Базовый конструктор
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }


        /// <summary>
        /// проверка на одинаковость точек
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        private static bool Equals(Point obj1, Point obj2)
        {
            if (obj1.X == obj2.X && obj1.Y == obj2.Y)
                return true;
            return false;
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator ==(Point obj1, Point obj2)
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
        public static bool operator !=(Point obj1, Point obj2)
        {
            if (Equals(obj1, obj2))
                return false;
            return true;
        }


        /// <summary>
        /// Переопределенный метод соответствия
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {            
            return Equals(obj as Point);
        }

        /// <summary>
        /// Переопределенный Хэшкод
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 352033288;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

    }
}
