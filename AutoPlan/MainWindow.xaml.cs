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
            Rect rect1 = new Rect();
            rect1.Location = new System.Windows.Point(10, 5);
            rect1.Size = new Size(200, 50);
            Rectangle r1 = new Rectangle(new Point(24, 16), new Point(56, 57));
            Rectangle r2 = new Rectangle(new Point(-14, 53), new Point(24, 58));
            bool res = r1.IntersectWith(r2);
            bool res2 = r2.IntersectWith(r1);
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

        }

        private void testpress_Click(object sender, RoutedEventArgs e)
        {
            testc();
        }
    }
}
