using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class LinearData
    {
        public double Slope { get; set; }
        public double StdDev { get; set; }
        public string XY { get; set; }

        public LinearData(double slope, double stdDev, string xy)
        {
            Slope = slope;
            StdDev = stdDev;
            XY = xy;
        }
    }
}
