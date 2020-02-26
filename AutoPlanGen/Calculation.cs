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
        /// Положение штурвалов
        /// </summary>
        public enum Transform
        {
            Bottom = 0,
            Left = 1,
            Top = 2,
            Right = 3
        }

        /// <summary>
        /// Возвращает трансформированный прямоугольник до расстановки стеллажей
        /// </summary>
        /// <param name="BaseRectangle"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static Rectangle TransformForward(Rectangle BaseRectanglePos, Transform Position, bool ShturvalOut = true)
        {
            // длина штурвала
            int ShturvalLength = 165;
            Rectangle BaseRectangle;
            if (ShturvalOut)
            {
                BaseRectangle = new Rectangle(BaseRectanglePos.BottomLeft, BaseRectanglePos.TopRight);
            }
            else
            {
                BaseRectangle = new Rectangle(
                        new Point(BaseRectanglePos.BottomLeft.X, BaseRectanglePos.BottomLeft.Y + ShturvalLength), BaseRectanglePos.TopRight);
            }

            if (Position == Transform.Bottom)
            {
                return BaseRectangle;
            }

            // точка поворота
            Point pointRot = new Point(0, 0);
            if (Position == Transform.Left)
            {
                if (!ShturvalOut)
                    BaseRectangle = new Rectangle(
                        new Point(BaseRectanglePos.BottomLeft.X + ShturvalLength, BaseRectanglePos.BottomLeft.Y), BaseRectanglePos.TopRight);
                //Point pointRot = new Point(0, 0);
                double Angle = Math.PI / 2;
                Point NewBottomLeft = RotatePoint(pointRot, BaseRectangle.BottomLeft, Angle);
                Point NewTopRight = RotatePoint(pointRot, BaseRectangle.TopRight, Angle);
                return new Rectangle(NewBottomLeft, NewTopRight);
            }

            if (Position == Transform.Top)
            {
                if (!ShturvalOut)
                    BaseRectangle = new Rectangle(
                        BaseRectanglePos.BottomLeft, new Point(BaseRectanglePos.TopRight.X, BaseRectanglePos.TopRight.Y - ShturvalLength));

                double Angle = Math.PI / 2;
                Point NewBottomLeft = RotatePoint(pointRot, BaseRectangle.BottomLeft, Angle);
                Point NewTopRight = RotatePoint(pointRot, BaseRectangle.TopRight, Angle);
                Rectangle midpoint = new Rectangle(NewBottomLeft, NewTopRight);

                NewBottomLeft = RotatePoint(pointRot, midpoint.BottomLeft, Angle);
                NewTopRight = RotatePoint(pointRot, midpoint.TopRight, Angle);

                return new Rectangle(NewBottomLeft, NewTopRight);
            }

            if (Position == Transform.Right)
            {
                if (!ShturvalOut)
                    BaseRectangle = new Rectangle(
                        BaseRectanglePos.BottomLeft, new Point(BaseRectanglePos.TopRight.X - ShturvalLength, BaseRectanglePos.TopRight.Y));

                double Angle = Math.PI * 3 / 2;
                Point NewBottomLeft = RotatePoint(pointRot, BaseRectangle.BottomLeft, Angle);
                Point NewTopRight = RotatePoint(pointRot, BaseRectangle.TopRight, Angle);
                return new Rectangle(NewBottomLeft, NewTopRight);
            }
            return BaseRectangle;
        }


        /// <summary>
        /// Поворачивает точку вокруг заданной точки на заданный угол
        /// </summary>
        /// <param name="pointRot">Точка вокруг которой вращаем</param>
        /// <param name="PointBase">Точка, которую надо повернуть</param>
        /// <param name="Angle">Угол поворота (радианы)</param>
        /// <returns></returns>
        public static Point RotatePoint(Point pointRot, Point PointBase, double Angle)
        {
            double cos = Math.Cos(Angle);
            double sin = Math.Sin(Angle);
            //X =(pointRot.X + (point.X - pointRot.X) * cos - (point.Y - pointRot.Y) * sin)
            //Y = (pointRot.Y + (point.X - pointRot.X) * sin + (point.Y - pointRot.Y) * cos)
            double BX = pointRot.X + (PointBase.X - pointRot.X) * cos - (PointBase.Y - pointRot.Y) * sin;
            double BY = pointRot.Y + (PointBase.X - pointRot.X) * sin - (PointBase.Y - pointRot.Y) * cos;
            return new Point(BX, BY);


        }
        /// <summary>
        /// Трансформирует обратно расставленные секции
        /// </summary>
        /// <param name="Unrotated"></param>
        /// <param name="Position"></param>
        /// <returns></returns>
        public static List<Section> TransforSection(List<Section> Unrotated, Transform Position)
        {
            if (Position == Transform.Bottom)
                return Unrotated;
            double Angle = 0;
            List<Section> Rotated = new List<Section>();
            if (Position == Transform.Top)
            {
                //throw new NotImplementedException();

                Angle = -Math.PI / 2;

                Point pointRot = new Point(0, 0);
                foreach (Section Item in Unrotated)
                {
                    Point NewBottomLeft = RotatePoint(pointRot, Item.BottomLeft, Angle);
                    NewBottomLeft = RotatePoint(pointRot, NewBottomLeft, Angle);
                    Section toAdd = new Section(Item, NewBottomLeft);
                    toAdd.Rotation = -Math.PI;
                    Rotated.Add(toAdd);
                }
            }
            else
            {
                if (Position == Transform.Left)
                    Angle = -Math.PI / 2;
                if (Position == Transform.Right)
                    Angle = -Math.PI * 3 / 2;


                Point pointRot = new Point(0, 0);
                foreach (Section Item in Unrotated)
                {
                    Point NewBottomLeft = RotatePoint(pointRot, Item.BottomLeft, Angle);
                    Section toAdd = new Section(Item, NewBottomLeft);
                    toAdd.Rotation = Angle;
                    Rotated.Add(toAdd);
                }
            }

            return Rotated;

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

            retVal = retVal.OrderByDescending(n => n).ToList();
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


        /// <summary>
        /// Формирует список секций на основе перечня стеллажей и выбранных данных
        /// </summary>
        /// <param name="FullSectionList">Перечень секций полный</param>
        /// <param name="RoomArea">Прямоугольник в который необходимо вписать стеллажи</param>
        /// <param name="ShelfLengthMin">минимальная длина полки</param>
        /// <param name="ShelfLengthMax">Максимальная длина полки</param>
        /// <param name="StellarHeight">Высота стеллажа</param>
        /// <param name="ShelfWidth">Глубина полки</param>
        /// <param name="WorkPass">Рабочий проход</param>
        /// <param name="LeftStat">Стационар слева в системе передвижных</param>
        /// <param name="RightStat">Стационар справа в системе передвижных</param>
        /// <param name="DoubleSidedStat">Стационары двухсторонние или односторонние</param>
        /// <param name="FalseFloor">Фальшпол</param>
        /// <param name="LengthLimit">Ограничения длины ряда</param>
        /// <param name="LengthLimitNumber">Максимальное количество секций до стационара при ограничении длины ряда</param>
        /// <returns></returns>
        public static List<Section> GetStellar(List<Section> FullSectionList, Rectangle RoomArea, double ShelfLengthMin, double ShelfLengthMax, int StellarHeight,
    double ShelfWidth, double WorkPass, bool LeftStat, bool RightStat, bool DoubleSidedStat, bool FalseFloor, bool LengthLimit, int LengthLimitNumber)
        {
            // финальный перечень стеллажей
            List<Section> Complete = new List<Section>();

            if (RoomArea == null)
            {
                return Complete;
            }
            // перечень допустимых стационаров
            List<Section> AllowedStat = new List<Section>();

            // помещение для двухсторонних стеллажей
            Rectangle roomForDoubleSided = new Rectangle(RoomArea.BottomLeft, RoomArea.TopRight);
            double WorkPassLength = WorkPass;

            // если нет стационаров по краям
            if (!LeftStat && !RightStat)
            {
                roomForDoubleSided = Calculation.getAreaWithoutWorkPassage(RoomArea, WorkPassLength);
            }
            // если есть стационары по краям
            else
            {
                if (DoubleSidedStat)
                {
                    // если стационары двухсторонние
                    AllowedStat = FullSectionList.Where(t => t.FakeLength >= ShelfLengthMin
                        && t.FakeLength <= ShelfLengthMax && t.SecHeight == StellarHeight && t.FakeWidth == ShelfWidth && t.Double && t.Stationary).ToList();
                }
                else
                {
                    // если стационары односторонние
                    AllowedStat = FullSectionList.Where(t => t.FakeLength >= ShelfLengthMin
                        && t.FakeLength <= ShelfLengthMax && t.SecHeight == StellarHeight && t.FakeWidth == ShelfWidth && !t.Double && t.Stationary).ToList();
                }

                // определяем глубину стационара двухстороннего для заданных параметров
                double StatWidth = 0;
                // если в выборке есть стационары
                if (AllowedStat.Count > 0)
                {
                    StatWidth = AllowedStat.Select(t => t.Length).Max();
                }

                // проверяем на допустимость размеров помещения
                if ((RoomArea.Length < StatWidth && !DoubleSidedStat) || ((RoomArea.Length < (StatWidth * 2 + WorkPassLength) && DoubleSidedStat)))
                {
                    return Complete;
                }

                // если стационары слева
                if (LeftStat && !RightStat)
                {
                    // задаем рамки для помещения под стационары
                    Rectangle forLeftStat = new Rectangle(RoomArea.BottomLeft, new Point(RoomArea.BottomLeft.X + StatWidth, RoomArea.TopRight.Y));
                    // добавляем стационары в массив
                    Complete.AddRange(Calculation.SectionPack(forLeftStat, AllowedStat));

                    // вычисляем оставшееся помещения для передвижных стеллажей
                    Rectangle RemainedRoom = new Rectangle(new Point(RoomArea.BottomLeft.X + StatWidth, RoomArea.BottomLeft.Y), RoomArea.TopRight);
                    roomForDoubleSided = Calculation.getAreaWithoutWorkPassage(RemainedRoom, WorkPassLength);
                }

                // если стационары справа
                if (RightStat && !LeftStat)
                {
                    // задаем рамки для помещения под стационары
                    Rectangle forRightStat = new Rectangle(new Point(RoomArea.BottomRight.X - StatWidth, RoomArea.BottomLeft.Y), RoomArea.TopRight);
                    // добавляем стационары в массив
                    Complete.AddRange(Calculation.SectionPack(forRightStat, AllowedStat));
                    // вычисляем оставшееся помещения для передвижных стеллажей
                    Rectangle RemainedRoom = new Rectangle(RoomArea.BottomLeft, new Point(RoomArea.TopRight.X - StatWidth, RoomArea.TopRight.Y));
                    roomForDoubleSided = Calculation.getAreaWithoutWorkPassage(RemainedRoom, WorkPassLength);
                }

                // если стационары слева и справа
                if (RightStat && LeftStat)
                {
                    // задаем рамки для левого помещения под стационары
                    Rectangle forLeftStat = new Rectangle(RoomArea.BottomLeft, new Point(RoomArea.BottomLeft.X + StatWidth, RoomArea.TopRight.Y));
                    // добавляем стационары в массив
                    Complete.AddRange(Calculation.SectionPack(forLeftStat, AllowedStat));
                    // задаем рамки для правого помещения под стационары
                    Rectangle forRightStat = new Rectangle(new Point(RoomArea.BottomRight.X - StatWidth, RoomArea.BottomLeft.Y), RoomArea.TopRight);
                    // добавляем стационары в массив
                    Complete.AddRange(Calculation.SectionPack(forRightStat, AllowedStat));
                    // вычисляем оставшееся помещения для передвижных стеллажей
                    Rectangle RemainedRoom = new Rectangle(new Point(RoomArea.BottomLeft.X + StatWidth, RoomArea.BottomLeft.Y),
                        new Point(RoomArea.TopRight.X - StatWidth, RoomArea.TopRight.Y));
                    roomForDoubleSided = Calculation.getAreaWithoutWorkPassage(RemainedRoom, WorkPassLength);
                }
            }


            // Основные передвижные стеллажи
            List<Section> AllowedItems = new List<Section>();
            if (!FalseFloor)
                AllowedItems =
                FullSectionList.Where(t => t.FakeLength >= ShelfLengthMin
                && t.FakeLength <= ShelfLengthMax && t.SecHeight == StellarHeight && t.FakeWidth == ShelfWidth && t.Double && !t.Stationary && !t.FalseFloor).ToList();
            else
                AllowedItems =
               FullSectionList.Where(t => t.FakeLength >= ShelfLengthMin
               && t.FakeLength <= ShelfLengthMax && t.SecHeight == StellarHeight && t.FakeWidth == ShelfWidth && t.Double && !t.Stationary && t.FalseFloor).ToList();

            // Теперь разбираемся с секциями по делению по краям
            // рабочий проход у нас остался справа
            if (!LengthLimit)
            {
                Complete.AddRange(Calculation.SectionPack(roomForDoubleSided, AllowedItems));
            }
            else
            {
                // определяем количество стеллажей
                Section Wid = AllowedItems.FirstOrDefault();
                double SectionLength = Wid.Length;
                if (roomForDoubleSided.Length / SectionLength > LengthLimitNumber)
                {
                    AllowedStat = FullSectionList.Where(t => t.FakeLength >= ShelfLengthMin
                            && t.FakeLength <= ShelfLengthMax && t.SecHeight == StellarHeight && t.FakeWidth == ShelfWidth && t.Double && t.Stationary).ToList();
                    double StatWidth = 0;
                    // если в выборке есть стационары
                    if (AllowedStat.Count > 0)
                    {
                        StatWidth = AllowedStat.Select(t => t.Length).Max();
                    }

                    // здесь надо придумать методологию деления на секции и проходы, учитывая что последний проход рабочий уже есть
                    //Complete.AddRange(Calculation.SectionPack(roomForDoubleSided, AllowedItems));
                    if ((roomForDoubleSided.Length) / SectionLength > LengthLimitNumber && (roomForDoubleSided.Length - WorkPassLength) / SectionLength <= LengthLimitNumber * 2 + 1)
                    {
                        // если больше заданного количества секций но меньше чем два подряд

                        LessThanTwo(Complete, AllowedStat, roomForDoubleSided, WorkPassLength, AllowedItems, SectionLength, StatWidth);
                    }
                    else
                    {
                        // если входит больше двух полных комплектов
                        double RemainedLength = roomForDoubleSided.Length;
                        do
                        {
                            double FirstLength = SectionLength * LengthLimitNumber;
                            double firstPointX = roomForDoubleSided.BottomLeft.X + (roomForDoubleSided.Length - RemainedLength);
                            // помещение для мобильных стеллажей
                            Rectangle MobileStellar = new Rectangle(
                                new Point(firstPointX, roomForDoubleSided.BottomLeft.Y),
                                new Point(firstPointX + FirstLength, roomForDoubleSided.TopRight.Y)
                                );
                            // мобильные стеллажи
                            Complete.AddRange(Calculation.SectionPack(MobileStellar, AllowedItems));
                            // помещение для стационаров
                            Rectangle StatStellar = new Rectangle(
                                new Point(firstPointX + FirstLength + WorkPassLength, roomForDoubleSided.BottomLeft.Y),
                                new Point(firstPointX + FirstLength + WorkPassLength + StatWidth + 1, roomForDoubleSided.TopRight.Y)
                                );
                            Complete.AddRange(Calculation.SectionPack(StatStellar, AllowedStat));
                            RemainedLength -= FirstLength + WorkPassLength + StatWidth;

                        }
                        while ((RemainedLength - WorkPassLength) / SectionLength > LengthLimitNumber * 2 + 1);
                        Rectangle RemainedRoom = new Rectangle(
                                new Point(roomForDoubleSided.BottomLeft.X + (roomForDoubleSided.Length - RemainedLength), roomForDoubleSided.BottomLeft.Y),
                                roomForDoubleSided.TopRight
                                );
                        if (RemainedLength > LengthLimitNumber * SectionLength && (RemainedLength - WorkPassLength) <= (LengthLimitNumber * 2 + 1) * SectionLength)
                        {
                            LessThanTwo(Complete, AllowedStat, RemainedRoom, WorkPassLength, AllowedItems, SectionLength, StatWidth);
                        }
                        else
                        {
                            Complete.AddRange(Calculation.SectionPack(RemainedRoom, AllowedItems));
                        }
                    }
                }
                else
                {
                    Complete.AddRange(Calculation.SectionPack(roomForDoubleSided, AllowedItems));
                }

            }
            return Complete;
            //List<Section> Vert = Calculation.SectionPack(roomForDoubleSided, AllowedItems);
        }

        // Уменьшение 
        private static void LessThanTwo(List<Section> Complete, List<Section> AllowedStat, Rectangle roomForDoubleSided, double WorkPassLength, List<Section> AllowedItems, double SectionLength, double StatWidth)
        {
            // делим ряд на два

            // вычисляем длину по секциям за вычетом рабочего прохода
            double remainedLength = roomForDoubleSided.Length - WorkPassLength - StatWidth;
            // если стационары двухсторонние
            int TotalStellCount = Convert.ToInt32(remainedLength / SectionLength);
            int HalfStelCount = TotalStellCount / 2;
            double firstLength = HalfStelCount * SectionLength;
            // первая часть
            Rectangle FirstArea = new Rectangle(roomForDoubleSided.BottomLeft,
                new Point(roomForDoubleSided.BottomLeft.X + firstLength + 1, roomForDoubleSided.TopRight.Y));
            Complete.AddRange(Calculation.SectionPack(FirstArea, AllowedItems));
            // стационар двухсторонний
            Rectangle CenterStationary = new Rectangle(
                new Point(roomForDoubleSided.BottomLeft.X + firstLength + WorkPassLength, roomForDoubleSided.BottomLeft.Y),
                new Point(roomForDoubleSided.BottomLeft.X + firstLength + WorkPassLength + StatWidth + 1, roomForDoubleSided.TopRight.Y));
            Complete.AddRange(Calculation.SectionPack(CenterStationary, AllowedStat));
            // оставшаяся часть
            Rectangle RemainedArea = new Rectangle(
                new Point(roomForDoubleSided.BottomLeft.X + firstLength + WorkPassLength + StatWidth, roomForDoubleSided.BottomLeft.Y),
                roomForDoubleSided.TopRight
                );
            Complete.AddRange(Calculation.SectionPack(RemainedArea, AllowedItems));
        }
    }
}
