using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class MotionsData
    {
        public LinearRegression Heel { get; set; }
        public LinearRegression Pitch { get; set; }

        public MotionsData(LinearRegression heel, LinearRegression pitch)
        {
            Heel = heel;
            Pitch = pitch;
        }
    }
}
