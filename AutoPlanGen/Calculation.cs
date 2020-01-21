using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    public class Calculation
    {
        /// <summary>
        /// Возвращает линейный раскрой из элементов
        /// по максимальной длине спереди
        /// </summary>
        /// <param name="TotalLength">Полная длина элементов</param>
        /// <param name="Variants">Варианты длины</param>
        /// <returns></returns>
        public static List<double> getRowByDistance(double TotalLength, List<double> Variants)
        {
            // made by Pulkovo Aeroflot Lounge
            List<double> retVal = new List<double>();
            if (Variants.Count == 0)
                return retVal;
            // Определяем максимальную длину
            double MaxLength = Variants.Select(n => n).Max();
            // если максимальная длина больше чем весь отрезок
            if (MaxLength > TotalLength)
            {
                // перебираем элементы для определения минимальной максимальной длины впиываемой в заданный отрезок
                List<double> DistinctElements = Variants.Select(n => n).Distinct().OrderBy(n => n).ToList();

                // флаг отсутствия подобающего по длине элемента в списке
                bool ShortFlag = true;
                for (int i = 0; i < DistinctElements.Count; i++)
                {
                    if (DistinctElements[i] <= TotalLength)
                    {
                        // и присваиваем её MaxLength
                        MaxLength = DistinctElements[i];
                        ShortFlag = false;
                        break;
                    }
                }
                // если ни одна длина не вписывается возвращаем пустое значение
                if (ShortFlag)
                    return retVal;
            }
            // определяем сколько элементов максимальной длины войдет
            int SimpleMaxCount = Convert.ToInt32(Math.Truncate(TotalLength / MaxLength));
            // вычисляем остаток
            double Ostatok = TotalLength - SimpleMaxCount * MaxLength;
            for (int i = 0; i < SimpleMaxCount; i++)
            {
                retVal.Add(MaxLength);
            }
            List<double> res1 = getRowByDistance(Ostatok, Variants);
            if (res1.Count != 0)
            {
                // могут еще войти элементы
                retVal.AddRange(res1);
            }

            return retVal;
        }

        /// <summary>
        /// Возвращает полную длину элементов массива
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="Absolute"></param>
        /// <returns></returns>
        public static double TotalLength(List<double> ItemList, bool Absolute = true)
        {
            double resultat = 0;
            foreach (double Item in ItemList)
            {
                if (Absolute)
                    resultat += Math.Abs(Item);
                else
                    resultat += Item;
            }
            return resultat;
        }


    }
}
