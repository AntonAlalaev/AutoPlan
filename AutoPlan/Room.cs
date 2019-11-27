using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    class Room:Rectangle
    {
        public Room(Point BottomLeft, Point BottomRight, Point TopLeft, Point TopRight):base(BottomLeft, BottomRight, TopLeft, TopRight)
        {
        }

    }
}
