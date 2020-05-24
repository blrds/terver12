using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
   public class myPoint
    {
        public double x { get; set; }
        public double y { get; set; }
        public string toString()
        {
            return x.ToString() + " " + y.ToString();
        }

        public int ComparisonbyX(myPoint a, myPoint b)
        {
            if (a.x == b.x) return 0;
            if (a.x > b.x) return 1;
            else return -1;
        }

        public int ComparisonbyY(myPoint a, myPoint b)
        {
            if (a.y == b.y) return 0;
            if (a.y > b.y) return 1;
            else return -1;
        }
    }
}
