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
        /// Возвращает прямоугольник за вычетом рабочего прохода
        /// </summary>
        /// <param name="Space">Исходный прямоугольник</param>
        /// <param name="PassageLength">Ширина рабочего прохода (по X)</param>
        /// <returns></returns>
        public static Rectangle getAreaWithoutWorkPassage(Rectangle Space, double PassageLength)
        {
            return new Rectangle(Space.BottomLeft, new Point(Space.TopRight.X - PassageLength, Space.TopRight.Y));
        }

        /// <summary>
        /// Заполняет секциями заданную площадь
        /// </summary>
        /// <param name="freeSpace">Площадь</param>
        /// <param name="AllowedItems">Допустимый набор секций одной глубины и высоты</param>
        /// <returns></returns>
        public static List<Section> SectionPack(Rectangle freeSpace, List<Section> AllowedItems)
        {
            // формирование реальных секций
            List<Section> VerticalLine = new List<Section>();

            // если помещение null
            if (freeSpace == null)
            {
                return VerticalLine;
            }

            if (AllowedItems == null)
            {
                return VerticalLine;
            }

            // если допустимых секций нет, то и вмещать в площадь нечего
            if (AllowedItems.Count == 0)
                return VerticalLine;

            // реальная глубина секции
            double RealWidth = AllowedItems.Select(t => t.Length).FirstOrDefault();

            // если глубина помещения меньше чем глубина одной секции
            // ни один стеллаж в заданное помещение не поместится
            if (freeSpace.Length < RealWidth)
                return VerticalLine;

            // допустимая длина основных секций
            List<double> AllowedMainLength = AllowedItems.Where(n => n.Main).Select(n => n.Height).Distinct().OrderBy(n => n).ToList();

            // допустимая длина дополнительных секций
            List<double> AllowedSecondLength = AllowedItems.Where(n => !n.Main).Select(n => n.Height).Distinct().ToList();


            // распределение по длинам секций
            List<double> finalLength = new List<double>();
            foreach (double MainLen in AllowedMainLength)
            {
                List<double> temp = Calculation.getRowByDistance(freeSpace.Height - MainLen, AllowedSecondLength);
                List<double> WithMain = new List<double>();
                WithMain.Add(MainLen);
                WithMain.AddRange(temp);
                if (Calculation.TotalLength(WithMain) >= Calculation.TotalLength(finalLength))
                    finalLength = WithMain;
            }

            // проверка на существование секций в заданную длину помещения
            if (finalLength.Count == 0)
                return VerticalLine;


            //Point Yposition = RoomData.BottomLeft;
            double Yposition = freeSpace.BottomLeft.Y;
            double Xposition = freeSpace.BottomLeft.X;

            // оставшаяся глбуина помещения
            double RemainedWidth = freeSpace.Length;

            do
            {
                // поиск  секций
                for (int i = 0; i < finalLength.Count; i++)
                {
                    // поиск основной секции
                    if (i == 0)
                    {
                        Section readyMain = AllowedItems.Where(n => n.Main
                            && n.Height == finalLength[i]).Select(n => n).FirstOrDefault();
                        if (readyMain == null)
                            return VerticalLine;
                        VerticalLine.Add(new Section(readyMain, new Point(Xposition, Yposition)));
                        Yposition += readyMain.Height;
                    }
                    // поиск дополнительных секций
                    else
                    {
                        Section readyDop = AllowedItems.Where(n => !n.Main
                           && n.Height == finalLength[i]).Select(n => n).FirstOrDefault();
                        if (readyDop == null)
                            return VerticalLine;
                        VerticalLine.Add(new Section(readyDop, new Point(Xposition, Yposition)));
                        Yposition += readyDop.Height;
                    }
                }
                Yposition = freeSpace.BottomLeft.Y;
                Xposition += RealWidth;
                RemainedWidth -= RealWidth;

            } while (RemainedWidth >= RealWidth);
            return VerticalLine;
        }






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
