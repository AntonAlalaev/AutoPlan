using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    /// <summary>
    /// Параметры системы
    /// </summary>
    class Parametrs
    {

        /// <summary>
        /// Глубина полки стеллажа
        /// </summary>
        public int StellarWidth { get; set; }
        
        /// <summary>
        /// Минимальная длина полки
        /// </summary>
        public int ShelfLengthMin { get; set; }
        
        /// <summary>
        /// Максимальная длина полки
        /// </summary>
        public int ShelfLengthMax { get; set; }
        
        /// <summary>
        /// Рабочий проход
        /// </summary>
        public int WorkPassage { get; set; }
        
        /// <summary>
        /// Расстояние до препядствия сверху
        /// </summary>
        public int ObstacleDistTop { get; set; }
        
        /// <summary>
        /// Расстояние до препядствия снизу
        /// </summary>
        public int ObstacleDistBottom { get; set; }
        
        /// <summary>
        /// Расстояние до препядствия слева
        /// </summary>
        public int ObstacleDistLeft { get; set; }

        /// <summary>
        /// Расстояние до препядствия справа
        /// </summary>
        public int ObstacleDistRight { get; set; }

        /// <summary>
        /// Расстояние до границы помещения сверху
        /// </summary>
        public int RoomDistTop { get; set; }

        /// <summary>
        /// Расстояние до границы помещения снизу
        /// </summary>
        public int RoomDistBottom { get; set; }

        /// <summary>
        /// Расстояние до границы помещения слева
        /// </summary>
        public int RoomDistLeft { get; set; }

        /// <summary>
        /// Расстояние до границы помещения справа
        /// </summary>
        public int RoomDistRight { get; set; }

    }

}
