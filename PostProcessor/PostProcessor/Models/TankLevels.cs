using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class TankLevels
    {
        public string TankFrom { get; set; }
        public double TankFromStartLevel { get; set; }
        public double TankFromEndLevel { get; set; }
        public string TankTo { get; set; }
        public double TankToStartLevel { get; set; }
        public double TankToEndLevel { get; set; }
    }
}
