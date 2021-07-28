using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class Draughts
    {        
        public double SF { get; set; }
        public double PF { get; set; }
        public double SA { get; set; }
        public double PA { get; set; }

        public Draughts(double sf, double pf, double sa, double pa)
        {
            SF = sf;
            PF = pf;
            SA = sa;
            PA = pa;
        }
    }
}
