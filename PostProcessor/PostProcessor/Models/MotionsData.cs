using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class MotionsData
    {
        public LinearData Heel { get; set; }
        public LinearData Pitch { get; set; }

        public MotionsData(LinearData heel, LinearData pitch)
        {
            Heel = heel;
            Pitch = pitch;
        }
    }
}
