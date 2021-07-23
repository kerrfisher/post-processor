
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class MotionsData
    {
        public int Number { get; set; }
        public LinearData Heel { get; set; }
        public LinearData Pitch { get; set; }

        public MotionsData(int number, LinearData heel, LinearData pitch)
        {
            Number = number;
            Heel = heel;
            Pitch = pitch;
        }
    }
}
