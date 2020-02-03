//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace AutoPlanGen
//{
//    /// <summary>
//    /// Логика взаимодействия для DialogWindow.xaml
//    /// </summary>
//    public partial class DialogWindow : Window
//    {
//        public DialogWindow()
//        {
//            InitializeComponent();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using AutoPlan;

namespace AutoPlanGen
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class DialogWindow : System.Windows.Window
    {


        //#pragma warning disable CA1303 // Не передавать литералы в качестве локализованных параметров
        List<Section> TotalSectionList;

        public static DialogWindow Current;

        public DialogWindow()
        {
            InitializeComponent();

            // загрузка данных секций
            string FileName = "LoadedSections.xml";
            TotalSectionList = Parametrs.LoadSection(FileName);

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
            RoomRectangle RoomData = new RoomRectangle(new Point(0, 0), new Point(8000, 6000));



            // если выбраны стационары

            double WorkPassLength = Convert.ToDouble(WorkPass.Text, CultureInfo.CurrentCulture);

            List<Section> ReturnSection = Calculation.GetStellar(TotalSectionList, RoomData, SelectedShelfLengthMin, SelectedShelfLengthMax, SelectedHeight, SelectedShelfWidth, WorkPassLength, LeftStat, RightStat, DoubleSidedStat);
            try
            {
                GenerateDrawing(ReturnSection, Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument, "C:\\stellar\\Shapes");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }

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

                MessageBox.Show(FindResource("SelectStellarDepth").ToString(), FindResource("ErrorCaption").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show(FindResource("SelectStellarHeight").ToString() + " " + SelectedHeight + " мм", FindResource("WarningCaption").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void testpress_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            testc();
        }

        private void InsertionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InsertSimple();
        }

        private void InsertSimple()
        {
            Autodesk.AutoCAD.Geometry.Scale3d Scale = new Autodesk.AutoCAD.Geometry.Scale3d();
           // MvBlockOps.CloneMvBlock("ПО 2065х1250х300", "C:\\stellar\\Shapes", "ПО 2065х1250х300.dwg", ref Scale);
            var activeDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            MvBlockPlacer.MvBlockRefInsert(activeDocument, "ПО 2065х1250х250", new Autodesk.AutoCAD.Geometry.Point3d(0, 0, 0), Scale, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SectionToPlace">Список секций для размещения</param>
        /// <param name="doc">Документ AutoCAD</param>
        /// <param name="PathToShapeFolder">Путь к папке с файлами</param>
        private void GenerateDrawing(List<Section> SectionToPlace, Autodesk.AutoCAD.ApplicationServices.Document doc, string PathToShapeFolder)
        {
            // сначала клонирем объекты БД

            List<string> UniqueName = SectionToPlace.Select(n => n.Name).Distinct().ToList();
            Dictionary<string, Autodesk.AutoCAD.Geometry.Scale3d> Scales = new Dictionary<string, Autodesk.AutoCAD.Geometry.Scale3d>();
            foreach (string Name in UniqueName)
            {
                Autodesk.AutoCAD.Geometry.Scale3d Scale = new Autodesk.AutoCAD.Geometry.Scale3d();
                MvBlockOps.CloneMvBlock(Name, PathToShapeFolder, Name + ".DWG", ref Scale, doc);
                Scales.Add(Name, Scale);
            }

            foreach (Section Item in SectionToPlace)
            {
                //Autodesk.AutoCAD.Geometry.Scale3d Scale = new Autodesk.AutoCAD.Geometry.Scale3d();
                //MvBlockOps.CloneMvBlock(Item.Name, PathToShapeFolder, Item.Name + ".DWG", ref Scale, doc);
                Autodesk.AutoCAD.Geometry.Scale3d Scale = Scales[Item.Name];
                MvBlockPlacer.MvBlockRefInsert(doc, Item.Name, new Autodesk.AutoCAD.Geometry.Point3d(Item.BottomLeft.X, Item.BottomLeft.Y, 0), Scale, 0);

            }

        }
    }
}
