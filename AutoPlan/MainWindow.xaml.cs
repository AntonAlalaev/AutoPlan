using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using OfficeOpenXml;
using System.IO;
using System.Xml;

namespace AutoPlan
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //#pragma warning disable CA1303 // Не передавать литералы в качестве локализованных параметров
        List<Section> TotalSectionList;
        public MainWindow()
        {
            InitializeComponent();
            // загрузка данных секций
            string FileName = "LoadedSections.xml";
            TotalSectionList = Parametrs.LoadSection(FileName);

            FalseFloorRB.IsChecked = true;


            // длины полок
            List<double> ShelfLength = TotalSectionList.Select(n => n.FakeLength).Distinct().OrderBy(n => n).ToList();

            Stationary.Items.Add("Нет");
            Stationary.Items.Add("Справа двухсторонний");
            Stationary.Items.Add("Слева двухсторонний");
            Stationary.Items.Add("Справа и слева двухсторонний");
            Stationary.Items.Add("Справа односторонний");
            Stationary.Items.Add("Слева односторонний");
            Stationary.Items.Add("Справа и слева односторонний");

            Stationary.SelectedIndex = 0;

            foreach (double Item in ShelfLength)
            {
                ShelfLengthMin.Items.Add(Item);
                ShelfLengthMax.Items.Add(Item);
            }

            // глубины полок
            List<double> ShelfWidth = TotalSectionList.Select(n => n.FakeWidth).Distinct().OrderBy(n => n).ToList();
            foreach (double Item in ShelfWidth)
            {
                StellarWidth.Items.Add(Item);
            }

            // высоты стеллажей
            List<int> StellarHeights = TotalSectionList.Select(n => n.SecHeight).Distinct().OrderBy(n => n).ToList();
            foreach (int Item in StellarHeights)
            {
                StellarHeight.Items.Add(Item);
            }
        }



        public void testc()
        {
            GetAllowedSelection(out double SelectedShelfLengthMin, out double SelectedShelfLengthMax, out int SelectedHeight, out double SelectedShelfWidth, out bool OK);
            if (!OK)
                return;

            AskStationary(out bool LeftStat, out bool RightStat, out bool DoubleSidedStat);

            // параметры помещения
            Rectangle RoomData = new Rectangle(new Point(0, 0), new Point(12000, 6000));

            // если выбраны стационары

            double WorkPassLength = Convert.ToDouble(WorkPass.Text, CultureInfo.CurrentCulture);

            //List<Section> ReturnSection = Calculation.GetStellar(TotalSectionList, RoomData, 
            //    SelectedShelfLengthMin, SelectedShelfLengthMax, SelectedHeight, SelectedShelfWidth, WorkPassLength, LeftStat, RightStat, DoubleSidedStat);


        }

        /// <summary>
        /// Возвращает коректные значения выбранных элементов
        /// </summary>
        /// <param name="SelectedShelfLengthMin"> минимальная длина секций</param>
        /// <param name="SelectedShelfLengthMax"> максимальная длина секций</param>
        /// <param name="SelectedHeight">Заданная высота секций</param>
        /// <param name="SelectedShelfWidth">Заданная глубина полки</param>
        private void GetAllowedSelection(out double SelectedShelfLengthMin, out double SelectedShelfLengthMax, out int SelectedHeight, out double SelectedShelfWidth, out bool OK)
        {
            // проверим на выбор глубины полки
            if (StellarWidth.SelectedItem == null)
            {

                MessageBoxResult result = MessageBox.Show(FindResource("SelectStellarDepth").ToString(), FindResource("ErrorCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                OK = false;
                SelectedShelfLengthMin = 0;
                SelectedShelfLengthMax = 0;
                SelectedHeight = 0;
                SelectedShelfWidth = 0;
            }
            else
            {
                SelectedShelfWidth = (double)StellarWidth.SelectedItem;
                if (ShelfLengthMin.SelectedItem == null)
                    SelectedShelfLengthMin = TotalSectionList.Select(n => n.FakeLength).Min();
                else
                    SelectedShelfLengthMin = (double)ShelfLengthMin.SelectedItem;
                if (ShelfLengthMax.SelectedItem == null)
                    SelectedShelfLengthMax = TotalSectionList.Select(n => n.FakeLength).Max();
                else
                    SelectedShelfLengthMax = (double)ShelfLengthMax.SelectedItem;
                if (StellarHeight.SelectedItem == null)
                {
                    SelectedHeight = TotalSectionList.Select(n => n.SecHeight).Min();
                    MessageBoxResult result = MessageBox.Show(FindResource("SelectStellarHeight").ToString() + " " + SelectedHeight + " мм", FindResource("WarningCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                    SelectedHeight = (int)StellarHeight.SelectedItem;
                OK = true;
            }
        }

        /// <summary>
        /// Возвращает корректные значения выбранных элементов в стационарах по краям
        /// </summary>
        /// <param name="Left">Наличие стационаров слева</param>
        /// <param name="Right">Наличие стационаров справа</param>
        /// <param name="DoubleSided">Двухсторонние или односторонние</param>
        private void AskStationary(out bool Left, out bool Right, out bool DoubleSided)
        {
            if (!Stationary.HasItems)
            {
                Left = false;
                Right = false;
                DoubleSided = false;
                return;
            }

            if (Stationary.SelectedIndex == 0)
            {
                Left = false;
                Right = false;
                DoubleSided = false;
                return;
            }
            if (Stationary.SelectedIndex == 1)
            {
                Left = false;
                Right = true;
                DoubleSided = true;
                return;
            }
            if (Stationary.SelectedIndex == 2)
            {
                Left = true;
                Right = false;
                DoubleSided = true;
                return;
            }
            if (Stationary.SelectedIndex == 3)
            {
                Left = true;
                Right = true;
                DoubleSided = true;
                return;
            }

            if (Stationary.SelectedIndex == 4)
            {
                Left = false;
                Right = true;
                DoubleSided = false;
                return;
            }
            if (Stationary.SelectedIndex == 5)
            {
                Left = true;
                Right = false;
                DoubleSided = false;
                return;
            }
            if (Stationary.SelectedIndex == 6)
            {
                Left = true;
                Right = true;
                DoubleSided = false;
                return;
            }
            Left = false;
            Right = false;
            DoubleSided = false;
            return;

        }

        private void testpress_Click(object sender, RoutedEventArgs e)
        {
            testc();
        }

        private void GenerateXLS_Click(object sender, RoutedEventArgs e)
        {
            // загружаем файл
            // StellarToExportXML.xlsx
            List<Section> Resultat = LoadFromExcel("StellarToExportXML.xlsx", 2);
            SaveSections("Output.xml", Resultat);
        }



        /// <summary>
        /// Сохраняет секции в XML файл
        /// </summary>
        /// <param name="FileName">Имя файла</param>
        public static void SaveSections(string FileName, List<Section> SectionData)
        {
            if (SectionData == null)
                return;
            if (SectionData.Count == 0)
                return;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("  ");
            // создаем
            XmlWriter xmlOut = XmlWriter.Create(FileName, settings);
            xmlOut.WriteStartDocument();
            xmlOut.WriteStartElement("Sections");
            foreach (Section Item in SectionData)
            {
                xmlOut.WriteStartElement("Section");
                xmlOut.WriteElementString(XMLParName.Name, Item.Name);
                xmlOut.WriteElementString(XMLParName.ShelfLength, Item.FakeLength.ToString());
                xmlOut.WriteElementString(XMLParName.ShelfWidth, Item.FakeWidth.ToString());
                xmlOut.WriteElementString(XMLParName.SectionRealLength, Item.Height.ToString());
                xmlOut.WriteElementString(XMLParName.SectionRealWidth, Item.Length.ToString());
                xmlOut.WriteElementString(XMLParName.SectionHeigh, Item.SecHeight.ToString());
                xmlOut.WriteElementString(XMLParName.FalseFloor, Item.FalseFloor.ToString());
                xmlOut.WriteElementString(XMLParName.MainSection, Item.Main.ToString());
                xmlOut.WriteElementString(XMLParName.DoubleSided, Item.Double.ToString());
                xmlOut.WriteElementString(XMLParName.Stationary, Item.Stationary.ToString());
                xmlOut.WriteEndElement();
            }
            xmlOut.WriteEndElement();
            xmlOut.Close();
        }

        /// <summary>
        /// Загружает секции из файла Excel
        /// </summary>
        /// <param name="FileName">Полный путь к файлу</param>
        /// <returns></returns>
        public static List<Section> LoadFromExcel(string FileName, int FirstRow = 1)
        {
            List<Section> retValue = new List<Section>();
            using (ExcelPackage p = new ExcelPackage())
            {
                using (FileStream fstream = File.OpenRead(FileName))
                {
                    p.Load(fstream);
                }
                int Sheets = p.Workbook.Worksheets.Count;
                ExcelWorksheet fsh = p.Workbook.Worksheets[1];
                int row = FirstRow;
                do
                {
                    ExcelRange Name = fsh.Cells[row, 1];
                    if (Name.Value == null)
                        break;
                    ExcelRange SectionHeight = fsh.Cells[row, 2];
                    if (SectionHeight.Value == null)
                        break;
                    ExcelRange ShelfLength = fsh.Cells[row, 3];
                    if (ShelfLength.Value == null)
                        break;
                    ExcelRange ShelfWidth = fsh.Cells[row, 4];
                    if (ShelfWidth.Value == null)
                        break;
                    ExcelRange SectionWidth = fsh.Cells[row, 5];
                    if (SectionWidth.Value == null)
                        break;
                    ExcelRange SectionLength = fsh.Cells[row, 6];
                    if (SectionLength.Value == null)
                        break;
                    ExcelRange Floor = fsh.Cells[row, 7];
                    if (Floor.Value == null)
                        break;
                    ExcelRange MainSec = fsh.Cells[row, 8];
                    if (MainSec.Value == null)
                        break;
                    ExcelRange DoubleSec = fsh.Cells[row, 9];
                    if (DoubleSec.Value == null)
                        break;
                    ExcelRange Stationary = fsh.Cells[row, 10];
                    if (Stationary == null)
                        break;
                    string strName = Name.Value.ToString();
#pragma warning disable CA1305 // Укажите IFormatProvider
                    Section toAdd = new Section(Name.Value.ToString(), Convert.ToInt32(SectionLength.Value.ToString()),
                        Convert.ToInt32(SectionWidth.Value.ToString()), Convert.ToInt32(ShelfLength.Value.ToString()),
                        Convert.ToInt32(ShelfWidth.Value.ToString()), Convert.ToInt32(SectionHeight.Value.ToString()),
                        Convert.ToBoolean(DoubleSec.Value.ToString()), Convert.ToBoolean(MainSec.Value.ToString()), 
                        Convert.ToBoolean(Stationary.Value.ToString()), Convert.ToBoolean(Floor.Value.ToString()));
#pragma warning restore CA1305 // Укажите IFormatProvider
                    retValue.Add(toAdd);
                    row++;
                }
                while (true);
            }
            return retValue;
        }

        private void ConnectdbBut_Click(object sender, RoutedEventArgs e)
        {
            StellarEnvironment envir = new StellarEnvironment();            
        }

        private void Rot_Click(object sender, RoutedEventArgs e)
        {
            Rectangle t1 = new Rectangle(new Point(4, 4), new Point(9, 15));


            // act
            Rectangle t2 = Calculation.TransformForward(t1, Calculation.Transform.Left);


            Rectangle p1 = new Rectangle(new Point(4, 4), new Point(9, 15));

            Rectangle p2 = Calculation.TransformForward(p1, Calculation.Transform.Top);
        }
    }
}
