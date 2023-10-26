using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indiva1
{
    public class Line
    {
        public Point leftP, rightP;

        public Line() { leftP = new Point(); rightP = new Point(); }

        public Line(Point l, Point r) { leftP = l; rightP = r; }

        public (Point, Point) GetPoints()
        {
            return (leftP, rightP);
        }

        public Point Diff()
        {
            return new Point(rightP.X - leftP.X, rightP.Y - leftP.Y);
        }
    }
}
