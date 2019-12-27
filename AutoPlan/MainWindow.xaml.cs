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
        public MainWindow()
        {
            InitializeComponent();
        }

        public void testc()
        {
            //Section test = new Section(1000, 600, new Point(0, 0));
            Rectangle r1 = new Rectangle(new Point(24, 16), new Point(56, 57));
            Rectangle r2 = new Rectangle(new Point(-14, 53), new Point(24, 58));
            RoomRectangle Area1 = new RoomRectangle(new Point(0, 0), new Point(57, 57));
            Area1.AddObstacle(r1);
            Area1.AddObstacle(r2);

            Polygon poly1 = new Polygon(new List<Point>()
            {
                new Point(1, 14), new Point(3, 8), new Point(8, 10), new Point(4.65, 4.2),
                new Point(13.92, -1.15), new Point(3, -2), new Point(0.08, -7.07),
                new Point(-2.95, -1.97), new Point(-11,0), new Point(-5,5), new Point(-8,12), new Point(-2.95,9.08)
            });

            Polygon thick = poly1.GetOffsetPolygon(5);

            poly1 = new Polygon(new List<Point>()
            {
                new Point(-1004.28, -464.3), new Point(-404.28, 35.7), new Point(-704.28, 735.7), new Point(-199.37, 444.19),
                new Point(195.72, 935.70), new Point(395.72, 335.7), new Point(895.72, 535.70),
                new Point(560.92, -44.21), new Point(1487.23,-579.01), new Point(395.72,-664.30), new Point(104.22,-1169.21), new Point(-191.54,-656.95)
            });

            Polygon poly2 = poly1.GetOffsetPolygon(5);

            Polygon poly3 = new Polygon(new List<Point>()
            {
                new Point(-1004.28, -464.3), new Point(-404.28, 35.7), new Point(-704.28, 735.7), new Point(-199.37, 444.19),
                new Point(195.72, 935.70), new Point(395.72, 335.7), new Point(895.72, 535.70),
                new Point(560.92, -44.21), new Point(1487.23,-579.01), new Point(395.72,-664.30), new Point(104.22,-1169.21), new Point(-191.54,-656.95)
            });
            List<double> res1 = Calculation.getRowByDistance(7000, new List<double> { 1250, 1000, 750 });

        }

        private void testpress_Click(object sender, RoutedEventArgs e)
        {
            testc();
        }
    }
}
