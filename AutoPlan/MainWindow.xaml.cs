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

            // перечень допустимых секций
            List<Section> AllowedItems = TotalSectionList.Where(t => t.FakeLength >= SelectedShelfLengthMin
                && t.FakeLength <= SelectedShelfLengthMax && t.SecHeight == SelectedHeight && t.FakeWidth == SelectedShelfWidth).ToList();
            // допустимая длина основных секций
            List<double> AllowedMainLength = AllowedItems.Where(n => n.Main).Select(n => n.Height).Distinct().OrderBy(n=>n).ToList();

            // допустимая длина дополнительных секций
            List<double> AllowedSecondLength = AllowedItems.Where(n => !n.Main).Select(n => n.Height).Distinct().ToList();

            
            
            // распределение по длинам секций
            List<double> finalLength = new List<double>();
            foreach (double MainLen in AllowedMainLength)
            {
                List<double> temp = Calculation.getRowByDistance(RoomData.Height - MainLen, AllowedSecondLength);
                List<double> WithMain = new List<double>();
                WithMain.Add(MainLen);
                WithMain.AddRange(temp);
                if (Calculation.TotalLength(WithMain) >= Calculation.TotalLength(finalLength))
                    finalLength = WithMain;
            }

            // формирование реальных секций
            List<Section> VerticalLine = new List<Section>();
            // проверка на существование секций
            if (finalLength.Count == 0)
                return;

            // поиск  секций
            for (int i = 0; i < finalLength.Count; i++)
            {
                // поиск основной секции
                if (i == 0)
                {
                    Section readyMain = AllowedItems.Where(n => n.Main
                        && n.SecHeight == SelectedHeight && n.FakeWidth == SelectedShelfWidth
                        && n.Height == finalLength[i]).Select(n => n).FirstOrDefault();
                    if (readyMain == null)
                        return;
                    VerticalLine.Add(readyMain);
                }
                // поиск дополнительных секций
                else
                { 
                    Section readyDop = AllowedItems.Where(n => !n.Main
                       && n.SecHeight == SelectedHeight && n.FakeWidth == SelectedShelfWidth
                       && n.Height == finalLength[i]).Select(n => n).FirstOrDefault();
                    if (readyDop == null)
                        return;
                    VerticalLine.Add(readyDop);
                }
            }




        }

        /// <summary>
        /// Возвращает коректные значения выбранных элементов
        /// </summary>
        /// <param name="SelectedShelfLengthMin"></param>
        /// <param name="SelectedShelfLengthMax"></param>
        /// <param name="SelectedHeight"></param>
        /// <param name="SelectedShelfWidth"></param>
        private void GetAllowedSelection(out double SelectedShelfLengthMin, out double SelectedShelfLengthMax, out int SelectedHeight, out double SelectedShelfWidth, out bool OK)
        {
            // проверим на выбор глубины полки
            if (StellarWidth.SelectedItem == null)
            {

                string MessageError = "Глубина стеллажа должна быть обязательно выбрана!";
                MessageBoxResult result = MessageBox.Show(MessageError, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    string MessageError = "Высота стеллажа не выбрана, по умолчанию будет выбрана минимальная " + SelectedHeight + " мм";
                    MessageBoxResult result = MessageBox.Show(MessageError, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
