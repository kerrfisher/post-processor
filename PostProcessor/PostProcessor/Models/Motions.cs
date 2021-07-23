using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class Motions
    {
        public double Heel { get; set; }
        public double Pitch { get; set; }

        public Motions(double heel, double pitch)
        {
            Heel = heel;
            Pitch = pitch;
        }
    }
}
