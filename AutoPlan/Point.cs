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
    }
}
