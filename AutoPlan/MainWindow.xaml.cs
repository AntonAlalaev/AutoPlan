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

            // длины полок
            List<double> ShelfLength = TotalSectionList.Select(n => n.FakeLength).Distinct().OrderBy(n => n).ToList();


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

            // параметры помещения
            RoomRectangle RoomData = new RoomRectangle(new Point(0, 0), new Point(12000, 6000));

            List<Section> AllowedItems = new List<Section>();
            // перечень допустимых секций
            //if (DoubleSection) // если двухсторонние
            //{
            AllowedItems = TotalSectionList.Where(t => t.FakeLength >= SelectedShelfLengthMin
                && t.FakeLength <= SelectedShelfLengthMax && t.SecHeight == SelectedHeight && t.FakeWidth == SelectedShelfWidth && t.Double == true).ToList();
            //}

            List<Section> Vert = Calculation.SectionPack(RoomData, AllowedItems);

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

        private void testpress_Click(object sender, RoutedEventArgs e)
        {
            testc();
        }
    }
}
