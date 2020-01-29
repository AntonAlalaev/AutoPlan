using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;


namespace AutoPlan
{
    /// <summary>
    /// Параметры системы
    /// </summary>
    public class Parametrs
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


        /// <summary>
        /// Загружает данные о геометрии секций из файла
        /// </summary>
        /// <param name="FileName">Имя файла XML</param>
        /// <returns></returns>
        public static List<Section> LoadSection(string FileName)
        {
            // перечень секций
            List<Section> retValue = new List<Section>();

            // объект XML
            XmlDocument xDoc = new XmlDocument();
            // считываем данные из файла
            xDoc.Load(FileName);
            // корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // для каждого элемента в xRoot
            foreach (XmlNode xnode in xRoot)
            {
                // название секции

                // параметры секции
                if (xnode.HasChildNodes)
                {
                    string StellarName = "";
                    // длина полки
                    int FormalLength = 0;
                    // глубина полки
                    int ShelfWidth = 0;
                    // высота секции
                    int Height = 0;
                    // действительная длина секции
                    int realLength = 0;
                    // ширина секции
                    int RealWidth = 0;
                    // фальшпол
                    bool falseFloor = false;
                    // основная секция
                    bool MainSection = false;
                    // двухсторонняя
                    bool DoubleSided = false;
                    // стационарная
                    bool Stationary = false;
                    foreach (XmlNode cnode in xnode)
                    {
                        // имя секции
                        if (cnode.Name == "Name")
                            StellarName = cnode.InnerText;
                        // Действительная длина секции
                        if (cnode.Name == XMLParName.SectionRealLength)
                            realLength = ParseToInt(cnode.InnerText);
                        // Действительная глубина секции
                        if (cnode.Name == XMLParName.SectionRealWidth)
                            RealWidth = ParseToInt(cnode.InnerText);
                        // Высота секции
                        if (cnode.Name == XMLParName.SectionHeigh)
                            Height = ParseToInt(cnode.InnerText);
                        // Глубина полки
                        if (cnode.Name == XMLParName.ShelfWidth)
                            ShelfWidth = ParseToInt(cnode.InnerText);
                        // Длина полки
                        if (cnode.Name == XMLParName.ShelfLength)
                            FormalLength = ParseToInt(cnode.InnerText);
                        // Фальшпол
                        if (cnode.Name == XMLParName.FalseFloor)
                            falseFloor = ParseToBool(cnode.InnerText);
                        // Основная секция
                        if (cnode.Name == XMLParName.MainSection)
                            MainSection = ParseToBool(cnode.InnerText);
                        // Стационарная
                        if (cnode.Name == XMLParName.Stationary)
                            Stationary = ParseToBool(cnode.InnerText);
                        // Двухсторонняя
                        if (cnode.Name == XMLParName.DoubleSided)
                            DoubleSided = ParseToBool(cnode.InnerText);
                    }
                    // добавляем секцию в список
                    retValue.Add(new Section(StellarName, realLength, RealWidth, FormalLength, ShelfWidth, Height, DoubleSided, MainSection, Stationary));
                }
            }
            return retValue;
        }

        /// <summary>
        /// Разбирает строку в целое число
        /// </summary>
        /// <param name="Value">Входная строка</param>
        /// <returns></returns>
        public static int ParseToInt(string Value)
        {
            if (Value == null)
                return 0;
            int result = 0;
            int.TryParse(Value, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out result);
            return result;
        }
        /// <summary>
        /// Разбирает строку в bool, 
        /// если число меньше или 0 то false, если больше 0 то true, 
        /// также разбирает совпадение с "true", без учета регистра,
        /// если не число если нет совпадения то false
        /// </summary>
        /// <param name="Value">Входная строка</param>
        /// <returns></returns>
        public static bool ParseToBool(string Value)
        {
            if (Value == null)
                return false;
            int intres = 0;
            if (!int.TryParse(Value, NumberStyles.Any, CultureInfo.GetCultureInfo("ru-RU"), out intres))
            {
                if (string.Compare(Value, 0, "true", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
                return false;
            }
            else
            {
                if (intres <= 0)
                    return false;
                else
                    return true;
            }
        }
    }

}
