using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class TankData
    {
        public int Number { get; set; }
        public string TankName { get; set; }
        public double LCG { get; set; }
        public double TCG { get; set; }
        public double Level { get; set; }
        public double Weight { get; set; }

        public TankData(int number, string tankName, double lcg, double tcg, double level, double weight)
        {
            Number = number;
            TankName = tankName;
            LCG = lcg;
            TCG = tcg;
            Level = level;
            Weight = weight;
        }
    }
}
