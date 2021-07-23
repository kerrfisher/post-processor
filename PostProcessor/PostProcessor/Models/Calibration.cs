using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class Calibration
    {
        public List<Motions> Motions { get; set; }
        public LinearData Heel { get; set; }
        public LinearData Pitch { get; set; }
    }
}
